namespace VSGBulgariaMarketplace.Application.Models.Order.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IOrderRepository : IRepository<Order, string>
    {
        public Order[] GetPendingOrders();

        public Order[] GetUserOrders(string email);

        public Order GetOrderItemIdAndQuantity(string id);

        public void Finish(string id);

        public void DeclineAllPendingOrdersWithDeletedItem(string itemId);

        public short GetPendingOrdersTotalItemQuantityByItemId(string itemId);
    }
}