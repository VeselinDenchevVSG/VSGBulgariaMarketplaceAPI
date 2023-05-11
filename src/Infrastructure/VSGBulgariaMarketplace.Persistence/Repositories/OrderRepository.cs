namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class OrderRepository : Repository<Order, int>, IOrderRepository
    {
        public OrderRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
            base.columnNamesString = "(ItemId, Quantity, Email, Status, CreatedAtUtc, ModifiedAtUtc, DeletedAtUtc, IsDeleted)";
            base.SetUpRepository(false);
        }

        public Order[] GetPendingOrders()
        {
            string sql =    "SELECT o.Quantity, o.CreatedAtUtc, o.ItemId AS ItemId, i.Id, i.Price FROM Orders AS o " +
                            "JOIN Items AS i ON o.ItemId = i.Id " +
                            "WHERE o.Status = 0 AND o.IsDeleted = 0 ";
            Order[] pendingOrders = base.DbConnection.Query<Order, Item, Order>(sql, (order, item) => 
            {
                order.Item = item;
                order.ItemId = item.Id;

                return order;
            }, 
            splitOn: "ItemId", transaction: this.Transaction).ToArray();

            return pendingOrders;
        }

        public Order[] GetUserOrders(string userId)
        {
            string sql = "SELECT o.Quantity, o.CreatedAtUtc, o.Status, o.ItemId AS ItemId, i.Id, i.Name, i.Price FROM Orders AS o " +
                            "JOIN Items AS i ON o.ItemId = i.Id " +
                            "WHERE o.IsDeleted = 0";
            Order[] userOrders = base.DbConnection.Query<Order, Item, Order>(sql, (order, item) =>
            {
                order.Item = item;
                order.ItemId = item.Id;

                return order;
            },
            splitOn: "ItemId", transaction: this.Transaction).ToArray();

            return userOrders;
        }
        
        public Order GetOrderItemIdAndQuantity(int id)
        {
            string sql = "SELECT ItemId, Quantity FROM Orders WHERE Id = @Id AND IsDeleted = 0";
            Order order = base.DbConnection.QueryFirstOrDefault<Order>(sql, new { Id = id }, transaction: this.Transaction);

            return order;
        }

        public void Finish(int id)
        {
            string sql = "UPDATE Orders SET Status = 1 WHERE Id = @Id";
            base.DbConnection.Execute(sql, new { Id = id }, transaction: this.Transaction);
        }
    }
}