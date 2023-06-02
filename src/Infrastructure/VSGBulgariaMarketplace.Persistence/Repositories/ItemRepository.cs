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
                                        "Description, Location, CreatedAtUtc, ModifiedAtUtc)";
            base.SetUpRepository();
        }
        
        public Item[] GetMarketplace()
        {
            string sql =    $"SELECT i.Id, i.Code, i.Price, i.Category, i.QuantityForSale, i.ImagePublicId, ci.Id AS CloudinaryImageId FROM Items AS i " +
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
            string sql = $"SELECT Id, Code, Name, Description, Category, QuantityForSale, Price, QuantityCombined, ImagePublicId, Location FROM Items";
            Item[] inventory = base.DbConnection.Query<Item>(sql, transaction: this.Transaction).ToArray();

            return inventory;
        }

        public Item GetById(string id)
        {
            string sql =    "SELECT i.Name, i.Price, i.Category, i.QuantityForSale, i.Description, i.ImagePublicId, ci.Id AS CloudinaryImageId FROM Items AS i " +
                            "LEFT JOIN CloudinaryImages AS ci " +
                            "ON i.ImagePublicId = ci.Id " +
                            "WHERE i.Id = @Id AND i.QuantityForSale IS NOT NULL";
            Item item = base.DbConnection.Query<Item, CloudinaryImage, Item>(sql, (item, image) =>
            {
                item.ImagePublicId = image.Id;

                return item;
            }, new { Id = id }, splitOn: "CloudinaryImageId", transaction: base.Transaction).FirstOrDefault();

            if (item?.ImagePublicId is not null)
            {
                item.ImagePublicId = item.ImagePublicId.Insert(0, CLOUDINARY_IMAGE_FOLDER);
            }

            return item;
        }

        public Item GetOrderItemInfoById(string id)
        {
            string sql = "SELECT Id, Code, Name, QuantityForSale, Price FROM Items WHERE Id = @Id";
            Item item = base.DbConnection.QueryFirstOrDefault<Item>(sql, new { Id = id }, transaction: base.Transaction);

            return item;
        }

        public void RequestItemPurchase(string id, short quantityRequested)
        {
            string getItemQuantityForSaleSql = "SELECT QuantityForSale FROM Items WHERE Id = @Id";
            string result = base.DbConnection.ExecuteScalar<string>(getItemQuantityForSaleSql, new { Id = id }, transaction: base.Transaction).ToString();
            short quantityForSale = short.Parse(result);

            if (quantityRequested <= quantityForSale)
            {
                string requestItemPurchaseSql = $"UPDATE Items SET QuantityForSale -= @QuantityRequested, ModifiedAtUtc = GETUTCDATE() WHERE Id = @Id";
                base.DbConnection.Execute(requestItemPurchaseSql, new { Id = id, QuantityRequested = quantityRequested }, transaction: base.Transaction);
            }
            else throw new ArgumentException("Not enough item quantity in stock!");
        }

        public void RestoreItemQuantities(string id, short quantity)
        {
            string sql = $"UPDATE Items SET QuantityForSale += @Quantity, QuantityCombined += @Quantity, ModifiedAtUtc = GETUTCDATE() WHERE Id = @Id";
            base.DbConnection.Execute(sql, new { Id = id, Quantity = quantity }, transaction: base.Transaction);
        }

        public void BuyItem(string id, short quantityRequested)
        {
            string sql = $"UPDATE Items SET QuantityCombined -= @QuantitySold, ModifiedAtUtc = GETUTCDATE() WHERE Id = @Id";
            base.DbConnection.Execute(sql, new { Id = id, QuantitySold = quantityRequested }, transaction: base.Transaction);
        }

        public void Update(string id, Item item)
        {
            string sql = $"UPDATE Items SET {this.parameterizedColumnsNamesUpdateString} WHERE Id = @Id";

            try
            {
                base.DbConnection.Execute(sql, new
                {
                    Id = id,
                    Code = item.Code,
                    Name = item.Name,
                    ImagePublicId = item.ImagePublicId,
                    Price = item.Price,
                    Category = item.Category,
                    QuantityCombined = item.QuantityCombined,
                    QuantityForSale = item.QuantityForSale,
                    Description = item.Description,
                    Location = item.Location,
                    ModifiedAtUtc = DateTime.UtcNow
                }, transaction: this.Transaction);
            }
            catch (SqlException se) when (se.Number == 2601)
            {
                ThrowEntityAlreadyExistsException(se.Message);
            }
        }

        public void Delete(string id)
        {
            string sql = $"DELETE FROM Items WHERE Id = @Id";
            bool hasBeenDeleted =
                Convert.ToBoolean(this.DbConnection.Execute(sql, new { Id = id }, transaction: this.Transaction));
            if (!hasBeenDeleted)
            {
                throw new NotFoundException($"Item doesn't exist!");
            }
        }

        public string GetItemPicturePublicId(string id)
        {
            string sql = "SELECT ImagePublicId FROM Items WHERE Id = @Id";
            var result = this.DbConnection.Query<string>(sql, new { Id = id }, base.Transaction);

            if (result.Count() == 0)
            {
                throw new NotFoundException($"Item doesn't exist!");
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

            try
            {
                base.Create(item);
            }
            catch (EntityAlreadyExistsException)
            {
                throw new EntityAlreadyExistsException("Item with the same name and location or code and location or all three the same already exists!");
            }

        }

        private static void ThrowEntityAlreadyExistsException(string message) 
        {
            string[] exceptionMessageTokens = message.Split(" _()'.".ToCharArray());
            string duplicateColumn = exceptionMessageTokens[17];
            string duplicateValue = exceptionMessageTokens[26];

            throw new EntityAlreadyExistsException($"Item with {duplicateColumn} {duplicateValue} already exists!");
        }
    }
}