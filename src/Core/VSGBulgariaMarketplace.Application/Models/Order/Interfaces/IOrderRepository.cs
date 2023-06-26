namespace VSGBulgariaMarketplace.Application.Models.Order.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IOrderRepository : IRepository<Order, string>
    {
        Order[] GetPendingOrders();

        Order[] GetUserOrders(string email);

        Order GetOrderItemIdAndQuantity(string id);

        void Finish(string id);

        Task DeclineAllPendingOrdersWithDeletedItemAsync(string itemId, CancellationToken cancellationToken);

        Task<short> GetPendingOrdersTotalItemQuantityByItemId(string itemId, CancellationToken cancellationToken);
    }
}