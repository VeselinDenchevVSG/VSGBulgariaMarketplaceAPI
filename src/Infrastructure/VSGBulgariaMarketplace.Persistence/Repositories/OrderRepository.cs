namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class OrderRepository : Repository<Order, int>, IOrderRepository
    {
        public OrderRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
            this.columnNamesString = base.columnNamesString.Replace("Item", "ItemId");

            this.parameterizedColumnsNamesString = base.columnNamesString.Replace("@Item", "@ItemId");

            base.insertSqlCommand = $"INSERT INTO Orders {this.columnNamesString} " +
                                    $"VALUES {base.parameterizedColumnsNamesString} ";
        }
    }
}
