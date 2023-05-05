namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemRepository : Repository<Item, int>, IItemRepository
    {
        public ItemRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            this.columnNamesString = base.columnNamesString.Replace("Category", "CategoryId")
                                                            .Replace("Orders", "OrderId");

            this.parameterizedColumnsNamesString = base.columnNamesString.Replace("@Category", "@CategoryId")
                                                                            .Replace("@Orders", "@OrderId");

            base.insertSqlCommand = $"INSERT INTO Orders {this.columnNamesString} " +
                                    $"VALUES {base.parameterizedColumnsNamesString} ";
        }
    }
}