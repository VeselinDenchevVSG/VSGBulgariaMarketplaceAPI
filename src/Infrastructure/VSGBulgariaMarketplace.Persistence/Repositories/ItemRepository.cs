namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using Microsoft.Data.SqlClient;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemRepository : Repository<Item, int>, IItemRepository
    {
        private const string CLOUDINARY_IMAGE_FOLDER = "VSG_Marketplace/";

        public ItemRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            base.columnNamesString = "(Id, Name, ImagePublicId, Price, Category, QuantityCombined, QuantityForSale, " +
                                        "Description, CreatedAtUtc, ModifiedAtUtc, DeletedAtUtc, IsDeleted)";
            base.SetUpRepository();
        }
        
        public Item[] GetMarketplace()
        {
            string sql =    $"SELECT i.Id, i.Price, i.Category, i.QuantityForSale, i.ImagePublicId, ci.Id AS CloudinaryImageId, ci.SecureUrl FROM Items AS i " +
                            $"JOIN CloudinaryImages AS ci " +
                            $"ON i.ImagePublicId = ci.Id " +
                            $"WHERE i.IsDeleted = 0 AND ci.IsDeleted = 0 AND i.QuantityForSale IS NOT NULL";
            Item[] marketplace = base.DbConnection.Query<Item, CloudinaryImage, Item>(sql, (item, image) =>
            {
                item.Image = image;

                return item;
            }, splitOn: "CloudinaryImageId", transaction: base.Transaction).ToArray();

            foreach (Item item in marketplace)
            {
                if (item.ImagePublicId is not null)
                {
                    item.ImagePublicId = item.ImagePublicId.Insert(0, CLOUDINARY_IMAGE_FOLDER);
                }
            }

            return marketplace;
        }

        public Item[] GetInventory()
        {
            string sql = $"SELECT Id, Name, Category, QuantityForSale, QuantityCombined FROM Items WHERE IsDeleted = 0";
            Item[] inventory = base.DbConnection.Query<Item>(sql, transaction: this.Transaction).ToArray();

            return inventory;
        }

        public Item GetByCode(int code)
        {
            string sql =    "SELECT i.Name, i.Price, i.Category, i.QuantityForSale, i.Description, i.ImagePublicId, ci.Id AS CloudinaryImageId, ci.SecureUrl FROM Items AS i " +
                            "JOIN CloudinaryImages AS ci " +
                            "ON i.ImagePublicId = ci.Id " +
                            "WHERE i.Id = @Code AND i.IsDeleted = 0 AND ci.IsDeleted = 0 AND i.QuantityForSale IS NOT NULL";
            Item item = base.DbConnection.Query<Item, CloudinaryImage, Item>(sql, (item, image) =>
            {
                item.Image = image;

                return item;
            }, new { Code = code }, splitOn: "CloudinaryImageId", transaction: base.Transaction).FirstOrDefault();

            if (item?.ImagePublicId is not null)
            {
                item.ImagePublicId = item.ImagePublicId.Insert(0, CLOUDINARY_IMAGE_FOLDER);
            }

            return item;
        }

        public Item GetQuantityForSaleAndPriceByCode(int code) 
        {
            string sql = "SELECT QuantityForSale, Price FROM Items WHERE Id = @Code AND IsDeleted = 0";
            Item item = base.DbConnection.QueryFirstOrDefault<Item>(sql, new { Code = code }, transaction: base.Transaction);

            return item;
        }

        public void BuyItem(int id, short quantity)
        {
            string getItemQuantityForSaleSql = "SELECT QuantityForSale FROM Items WHERE Id = @Id AND IsDeleted = 0";
            string result = base.DbConnection.ExecuteScalar(getItemQuantityForSaleSql, new { Id = id }, transaction: base.Transaction).ToString();
            short quantityForSale = short.Parse(result);

            if (quantity > quantityForSale)
            {
                throw new ArgumentOutOfRangeException("Not enough item quantity in stock!");
            }

            string buyItemSql = $"UPDATE Items SET QuantityForSale -= @QuantitySold, QuantityCombined -= @QuantitySold WHERE Id = @Id";
            base.DbConnection.Execute(buyItemSql, new { Id = id, QuantitySold = quantity }, transaction: base.Transaction);
        }

        public void Update(int code, Item item)
        {
            string sql = $"UPDATE Items SET {this.parameterizedColumnsNamesUpdateString} WHERE Id = @OldId";

            try
            {
                base.DbConnection.Execute(sql, new
                {
                    Id = item.Id,
                    Name = item.Name,
                    ImagePublicId = item.ImagePublicId,
                    Price = item.Price,
                    Category = item.Category,
                    QuantityCombined = item.QuantityCombined,
                    QuantityForSale = item.QuantityForSale,
                    Description = item.Description,
                    ModifiedAtUtc = DateTime.UtcNow,
                    OldId = code
                }, transaction: this.Transaction);
            }
            catch (SqlException se) when (se.Number == 2627)
            {
                base.ThrowPrimaryKeyViolationException(code);
            }
        }

        public string GetItemPicturePublicId(int code)
        {
            string sql = "SELECT ImagePublicId FROM Items WHERE Id = @Code";
            var result = this.DbConnection.Query<string>(sql, new { Code = code }, base.Transaction);

            if (result.Count() == 0)
            {
                throw new NotFoundException($"Item with code = {code} doesn't exist!");
            }

            string itemPicturePublicId = result.FirstOrDefault();

            if (itemPicturePublicId is not null)
            {
                itemPicturePublicId = CLOUDINARY_IMAGE_FOLDER + itemPicturePublicId;
            }

            return itemPicturePublicId;
        }
    }
}