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
            base.columnNamesString = "(Id, Name, PicturePublicId, Price, Category, QuantityCombined, QuantityForSale, " +
                                        "Description, CreatedAtUtc, ModifiedAtUtc, DeletedAtUtc, IsDeleted)";
            base.SetUpRepository();
        }
        
        public Item[] GetMarketplace()
        {
            string sql = $"SELECT Id, PicturePublicId, Price, Category, QuantityForSale FROM Items WHERE IsDeleted = 0";
            Item[] marketplace = base.DbConnection.Query<Item>(sql, transaction: base.Transaction).ToArray();

            foreach (Item item in marketplace)
            {
                if (item.PicturePublicId is not null)
                {
                    item.PicturePublicId = item.PicturePublicId.Insert(0, CLOUDINARY_IMAGE_FOLDER);
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
            string sql = "SELECT PicturePublicId, Name, Price, Category, QuantityForSale, Description FROM Items " +
                            "WHERE Id = @Code AND IsDeleted = 0";
            Item item = base.DbConnection.QueryFirstOrDefault<Item>(sql, new { Code = code }, transaction: base.Transaction);

            if (item?.PicturePublicId is not null)
            {
                item.PicturePublicId = item.PicturePublicId.Insert(0, CLOUDINARY_IMAGE_FOLDER);
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
                    PicturePublicId = item.PicturePublicId,
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
            string sql = "SELECT PicturePublicId FROM Items WHERE Id = @Code";
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