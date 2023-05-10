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
            string sql = $"SELECT Id, PicturePublicId, Price, Category, QuantityForSale FROM {this.tableName} WHERE IsDeleted = 0";
            Item[] marketplace = DbConnection.Query<Item>(sql, transaction: this.Transaction).ToArray();

            return marketplace;
        }

        public Item[] GetInventory()
        {
            string sql = $"SELECT Id, Name, Category, QuantityForSale, QuantityCombined FROM {this.tableName} WHERE IsDeleted = 0";
            Item[] inventory = DbConnection.Query<Item>(sql, transaction: this.Transaction).ToArray();

            return inventory;
        }

        public override void Update(int code, Item item)
        {
            string sql =    $"UPDATE {this.tableName} SET {this.parameterizedColumnsNamesUpdateString} WHERE Id = @OldId AND " +
                            $"IsDeleted = 0";
            bool hasBeenUpdated = Convert.ToBoolean(
                DbConnection.Execute(sql, new 
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