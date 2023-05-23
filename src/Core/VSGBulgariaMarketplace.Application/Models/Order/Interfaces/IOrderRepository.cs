namespace VSGBulgariaMarketplace.Application.Models.Order.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IOrderRepository : IRepository<Order, int>
    {
        public Order[] GetPendingOrders();

        public Order[] GetUserOrders(string email);

        public Order GetOrderItemIdAndQuantity(int id);

        public void Finish(int id);
    }
}