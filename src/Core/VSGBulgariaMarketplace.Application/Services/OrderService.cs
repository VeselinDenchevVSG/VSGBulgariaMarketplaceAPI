namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using Microsoft.Extensions.Caching.Memory;

    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class OrderService : BaseService<IOrderRepository, Order>, IOrderService
    {
        private IItemRepository itemRepository;

        public OrderService(IOrderRepository repository, IItemRepository itemRepository, IMemoryCache memoryCache, IMapper mapper) 
            : base(repository, memoryCache, mapper)
        {
            this.itemRepository = itemRepository;
        }

        public PendingOrderDto[] GetPendingOrders()
        {
            Order[] pendingOrders = base.repository.GetPendingOrders();
            PendingOrderDto[] pendingOrderDtos = base.mapper.Map<Order[], PendingOrderDto[]>(pendingOrders);

            return pendingOrderDtos;
        }

        public UserOrderDto[] GetUserOrders(string userId)
        {
            Order[] userOrders = base.repository.GetUserOrders(userId);
            UserOrderDto[] userOrderDtos = base.mapper.Map<Order[], UserOrderDto[]>(userOrders);

            return userOrderDtos;
        }

        public void Create(CreateOrderDto orderDto)
        {
            Order order = base.mapper.Map<CreateOrderDto, Order>(orderDto);

            Item item = this.itemRepository.GetQuantityForSaleAndPriceByCode(orderDto.ItemCode);

            bool isEnoughQuantity = orderDto.Quantity <= item.QuantityForSale;
            if (isEnoughQuantity)
            {
                order.Item = item;
                order.Email = "vdenchev@vsgbg.com";

                base.repository.Create(order);
            }
            else throw new ArgumentOutOfRangeException("Not enough item quantity for sale!");
        }

        public void Finish(int id)
        {
            Order order = base.repository.GetOrderItemIdAndQuantity(id);
            this.itemRepository.BuyItem(order.ItemId, order.Quantity);

            base.repository.Finish(id);
        }

        public void Decline(int id) => base.repository.Delete(id);
    }
}