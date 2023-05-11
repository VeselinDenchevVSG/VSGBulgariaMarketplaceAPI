namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemRepository : Repository<Item, int>, IItemRepository
    {
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
            Item[] marketplace = base.DbConnection.Query<Item>(sql, transaction: this.Transaction).ToArray();

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
                            "WHERE Id = @Code";
            Item item = base.DbConnection.QueryFirstOrDefault<Item>(sql, new { Code = code }, transaction: this.Transaction);

            return item;
        }

        public Item GetQuantityForSaleAndPriceByCode(int code) 
        {
            string sql = "SELECT QuantityForSale, Price FROM Items WHERE Id = @Code";
            Item item = base.DbConnection.QueryFirstOrDefault<Item>(sql, new { Code = code }, transaction: this.Transaction);

            return item;
        }

        public void BuyItem(int id, short quantity)
        {
            string getItemQuantityForSaleSql = "SELECT QuantityForSale FROM Items WHERE Id = @Id AND IsDeleted = 0";
            string result = base.DbConnection.ExecuteScalar(getItemQuantityForSaleSql, new { Id = id }).ToString();
            short quantityForSale = short.Parse(result);

            if (quantity > quantityForSale)
            {
                throw new ArgumentOutOfRangeException("Not enough item quantity in stock!");
            }

            string buyItemSql = $"UPDATE Items SET QuantityForSale -= @QuantitySold, QuantityCombined -= @QuantitySold WHERE Id = @Id";
            this.DbConnection.Execute(buyItemSql, new { Id = id, QuantitySold = quantity }, transaction: this.Transaction);
        }

        public void Update(int code, Item item)
        {
            string sql =    $"UPDATE {this.tableName} SET {this.parameterizedColumnsNamesUpdateString} WHERE Id = @OldId AND " +
                            $"IsDeleted = 0";
            bool hasBeenUpdated = Convert.ToBoolean(
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
                }, transaction: this.Transaction)
            );
            if (!hasBeenUpdated)
            {
                throw new ArgumentException($"Item with code = {code} doesn't exist!");
            }
        }
    }
}