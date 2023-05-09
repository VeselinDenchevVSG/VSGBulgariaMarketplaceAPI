namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    using static Dapper.SqlMapper;

    public class OrderRepository : Repository<Order, int>, IOrderRepository
    {
        public OrderRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
            this.columnNamesString = base.columnNamesString.Replace("Item, ", string.Empty);

            this.parameterizedColumnsNamesString = base.parameterizedColumnsNamesString.Replace("@Item, ", string.Empty);

            base.insertSqlCommand = $"INSERT INTO Orders {this.columnNamesString} " +
                                    $"VALUES {base.parameterizedColumnsNamesString} ";
        }

        public void Finish(int id)
        {
            string sql = "UPDATE Orders SET Status = 1 WHERE Id = @Id";
            DbConnection.Execute(sql, new { Id = id }, transaction: this.Transaction);
        }
    }
}
