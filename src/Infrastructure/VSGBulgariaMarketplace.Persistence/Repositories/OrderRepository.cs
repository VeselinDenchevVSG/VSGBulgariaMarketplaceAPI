namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class OrderRepository : Repository<Order, string>, IOrderRepository
    {
        public OrderRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
            base.columnNamesString = "(Id, ItemId, ItemCode, ItemName, ItemPrice, Quantity, Email, Status, CreatedAtUtc, ModifiedAtUtc)";
            base.SetUpRepository(false);
        }

        public Order[] GetPendingOrders()
        {
            string sql = "SELECT Id, ItemCode, Quantity, ItemPrice, Email, CreatedAtUtc FROM Orders " +
                            "WHERE Status = 0";
            Order[] pendingOrders = base.DbConnection.Query<Order>(sql, transaction: this.Transaction).ToArray();

            return pendingOrders;
        }

        public Order[] GetUserOrders(string email)
        {
            string sql =    "SELECT Id, ItemCode, ItemName, ItemPrice, Quantity, CreatedAtUtc, Status FROM Orders " +
                            "WHERE Email = @Email";
            Order[] userOrders = base.DbConnection.Query<Order>(sql, new { Email = email }, transaction: base.Transaction).ToArray();

            return userOrders;
        }
        
        public Order GetOrderItemIdAndQuantity(string id)
        {
            string sql = "SELECT ItemId, Quantity FROM Orders WHERE Id = @Id";
            Order order = base.DbConnection.QueryFirstOrDefault<Order>(sql, new { Id = id }, transaction: base.Transaction);

            return order;
        }

        public void Finish(string id)
        {
            string sql = "UPDATE Orders SET Status = 1, ModifiedAtUtc = GETUTCDATE() WHERE Id = @id";
            base.DbConnection.Execute(sql, new { Id = id }, transaction: base.Transaction);
        }

        public void DeclineAllPendingOrdersWithDeletedItem(string itemId)
        {
            string sql =    "SELECT o.Id, o.ItemId AS ItemId, i.Id FROM Orders AS o " +
                            "JOIN Items AS i " +
                            "ON o.ItemId = i.Id " +
                            "WHERE o.Status = 0 AND i.Id = @ItemId";
            Order[] pendingOrdersWithDeletedItem = base.DbConnection.Query<Order, Item, Order>(sql, (order, item) =>
            {
                order.ItemId = item.Id;

                return order;
            }, new { ItemId = itemId }, splitOn: "ItemId", transaction: base.Transaction).ToArray();

            foreach (Order order in pendingOrdersWithDeletedItem)
            {
                base.Delete(order.Id);
            }
        }

        public short GetPendingOrdersTotalItemQuantityByItemId(string itemId)
        {
            string sql = "SELECT SUM(Quantity) FROM Orders WHERE ItemId = @ItemId AND Status = 0";
            short pendingOrdersTotalItemQuantity = base.DbConnection.ExecuteScalar<short>(sql, new { ItemId = itemId }, transaction: base.Transaction);

            return pendingOrdersTotalItemQuantity;
        }

        public override void Create(Order order)
        {
            order.Id = Guid.NewGuid().ToString();

            base.Create(order);
        }
    }
}