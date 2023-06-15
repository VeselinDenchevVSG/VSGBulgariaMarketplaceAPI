namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Persistence.Constants;

    public class OrderRepository : Repository<Order, string>, IOrderRepository
    {
        public OrderRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
            base.columnNamesString = RepositoryConstant.ORDER_REPOSITORY_COLUMN_NAMES_STRING;
            base.SetUpRepository();
        }

        public Order[] GetPendingOrders()
        {
            string sql = RepositoryConstant.GET_PENDING_ORDERS_SQL_QUERY;
            Order[] pendingOrders = base.DbConnection.Query<Order>(sql, transaction: this.Transaction).ToArray();

            return pendingOrders;
        }

        public Order[] GetUserOrders(string email)
        {
            string sql = RepositoryConstant.GET_USER_ORDERS_SQL_QUERY;
            Order[] userOrders = base.DbConnection.Query<Order>(sql, new { Email = email }, transaction: base.Transaction).ToArray();

            return userOrders;
        }
        
        public Order GetOrderItemIdAndQuantity(string id)
        {
            string sql = RepositoryConstant.GET_ORDER_ITEM_ID_AND_QUANTITY;
            Order order = base.DbConnection.QueryFirstOrDefault<Order>(sql, new { Id = id }, transaction: base.Transaction);

            return order;
        }

        public void Finish(string id)
        {
            string sql = RepositoryConstant.FINISH_ORDER_SQL_QUERY;
            base.DbConnection.Execute(sql, new { Id = id }, transaction: base.Transaction);
        }

        public void DeclineAllPendingOrdersWithDeletedItem(string itemId)
        {
            string sql = RepositoryConstant.DECLINE_ALL_PENDING_ORDERS_WITH_DELETED_ITEM_SQL_QUERY;
            Order[] pendingOrdersWithDeletedItem = base.DbConnection.Query<Order, Item, Order>(sql, (order, item) =>
            {
                order.ItemId = item.Id;

                return order;
            }, new { ItemId = itemId }, splitOn: RepositoryConstant.ITEM_ID_ALIAS, transaction: base.Transaction).ToArray();

            foreach (Order order in pendingOrdersWithDeletedItem)
            {
                base.Delete(order.Id);
            }
        }

        public short GetPendingOrdersTotalItemQuantityByItemId(string itemId)
        {
            string sql = RepositoryConstant.GET_PENDING_ORDERS_TOTAL_ITEM_QUANTITY_BY_ITEM_ID_SQL_QUERY;
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