namespace VSGBulgariaMarketplace.Application.Models.Order.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;

    public interface IOrderService
    {
        public PendingOrderDto[] GetPendingOrders();

        public UserOrderDto[] GetUserOrders();

        public void Create(CreateOrderDto orderDto);

        public void Finish(int id);

        public void Decline(int id);
    }
}
