namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using Microsoft.Data.SqlClient;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemRepository : Repository<Item, string>, IItemRepository
    {
        private const string CLOUDINARY_IMAGE_FOLDER = "VSG_Marketplace/";

        public ItemRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            base.columnNamesString = "(Id, Code, Name, ImagePublicId, Price, Category, QuantityCombined, QuantityForSale, " +
                                        "Description, CreatedAtUtc, ModifiedAtUtc)";
            base.SetUpRepository();
        }
        
        public Item[] GetMarketplace()
        {
            string sql =    $"SELECT i.Code, i.Price, i.Category, i.QuantityForSale, i.ImagePublicId, ci.Id AS CloudinaryImageId FROM Items AS i " +
                            $"LEFT JOIN CloudinaryImages AS ci " +
                            $"ON i.ImagePublicId = ci.Id " +
                            $"WHERE i.QuantityForSale IS NOT NULL";
            Item[] marketplace = base.DbConnection.Query<Item, CloudinaryImage, Item>(sql, (item, image) =>
            {
                item.ImagePublicId = image.Id;

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
            string sql = $"SELECT Code, Name, Description, Category, QuantityForSale, Price, QuantityCombined, ImagePublicId FROM Items";
            Item[] inventory = base.DbConnection.Query<Item>(sql, transaction: this.Transaction).ToArray();

            return inventory;
        }

        public Item GetByCode(int code)
        {
            string sql =    "SELECT i.Name, i.Price, i.Category, i.QuantityForSale, i.Description, i.ImagePublicId, ci.Id AS CloudinaryImageId FROM Items AS i " +
                            "LEFT JOIN CloudinaryImages AS ci " +
                            "ON i.ImagePublicId = ci.Id " +
                            "WHERE i.Code = @Code AND i.QuantityForSale IS NOT NULL";
            Item item = base.DbConnection.Query<Item, CloudinaryImage, Item>(sql, (item, image) =>
            {
                item.ImagePublicId = image.Id;

                return item;
            }, new { Code = code }, splitOn: "CloudinaryImageId", transaction: base.Transaction).FirstOrDefault();

            if (item?.ImagePublicId is not null)
            {
                item.ImagePublicId = item.ImagePublicId.Insert(0, CLOUDINARY_IMAGE_FOLDER);
            }

            return item;
        }

        public Item GetOrderItemInfoByCode(int code)
        {
            string sql = "SELECT Id, Name, QuantityForSale, Price FROM Items WHERE Code = @Code";
            Item item = base.DbConnection.QueryFirstOrDefault<Item>(sql, new { Code = code }, transaction: base.Transaction);

            return item;
        }

        public void BuyItem(int code, short quantity)
        {
            string getItemQuantityForSaleSql = "SELECT QuantityForSale FROM Items WHERE Code = @Code";
            string result = base.DbConnection.ExecuteScalar(getItemQuantityForSaleSql, new { Code = code }, transaction: base.Transaction).ToString();
            short quantityForSale = short.Parse(result);

            if (quantity > quantityForSale)
            {
                throw new ArgumentOutOfRangeException("Not enough item quantity in stock!");
            }

            string buyItemSql = $"UPDATE Items SET QuantityForSale -= @QuantitySold, QuantityCombined -= @QuantitySold, ModifiedAtUtc = GETUTCDATE() WHERE Code = @Code";
            base.DbConnection.Execute(buyItemSql, new { Code = code, QuantitySold = quantity }, transaction: base.Transaction);
        }

        public void Update(int oldCode, Item item)
        {
            string sql = $"UPDATE Items SET {this.parameterizedColumnsNamesUpdateString} WHERE Code = @OldCode";

            try
            {
                base.DbConnection.Execute(sql, new
                {
                    Code = item.Code,
                    Name = item.Name,
                    ImagePublicId = item.ImagePublicId,
                    Price = item.Price,
                    Category = item.Category,
                    QuantityCombined = item.QuantityCombined,
                    QuantityForSale = item.QuantityForSale,
                    Description = item.Description,
                    ModifiedAtUtc = DateTime.UtcNow,
                    OldCode = oldCode
                }, transaction: this.Transaction);
            }
            catch (SqlException se) when (se.Number == 2601)
            {
                ThrowItemAlreadyExistsException(se.Message);
            }
        }

        public void DeleteByCode(int code)
        {
            string sql = $"DELETE FROM Items WHERE Code = @Code";
            bool hasBeenDeleted =
                Convert.ToBoolean(this.DbConnection.Execute(sql, new { Code = code }, transaction: this.Transaction));
            if (!hasBeenDeleted)
            {
                throw new NotFoundException($"{typeof(Item)} with code {code} doesn't exist!");
            }
        }

        public string GetItemPicturePublicId(int code)
        {
            string sql = "SELECT ImagePublicId FROM Items WHERE Code = @Code";
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

        public override void Create(Item item)
        {
            item.Id = Guid.NewGuid().ToString();

            base.Create(item);
        }

        private static void ThrowItemAlreadyExistsException(string message) 
        {
            string[] exceptionMessageTokens = message.Split(" _()'.".ToCharArray());
            string duplicateColumn = exceptionMessageTokens[17];
            string duplicateValue = exceptionMessageTokens[26];

            throw new ItemAlreadyExistsException($"Item with {duplicateColumn} {duplicateValue} already exists!");
        }
    }
}