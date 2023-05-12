namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class OrderService : BaseService<IOrderRepository, Order>, IOrderService
    {
        public const string PENDING_ORDERS_CACHE_KEY = "pending-orders";
        private const string USER_ORDER_CACHE_KEY_TEMPLATE = "orders-user-{0}";
        private const string FAKE_USER_ID = "1234"; // For debugging purposes I will use fake user id until we implement authentication

        private IItemRepository itemRepository;

        public OrderService(IOrderRepository repository, IItemRepository itemRepository, IMemoryCacheAdapter cacheAdapter, IMapper mapper) 
            : base(repository, cacheAdapter, mapper)
        {
            this.itemRepository = itemRepository;
        }

        public PendingOrderDto[] GetPendingOrders()
        {
            PendingOrderDto[] pendingOrderDtos = base.cacheAdapter.Get<PendingOrderDto[]>(PENDING_ORDERS_CACHE_KEY);
            if (pendingOrderDtos is null)
            {
                Order[] pendingOrders = base.repository.GetPendingOrders();
                pendingOrderDtos = base.mapper.Map<Order[], PendingOrderDto[]>(pendingOrders);

                base.cacheAdapter.Set(PENDING_ORDERS_CACHE_KEY, pendingOrderDtos);
            }

            return pendingOrderDtos;
        }

        public UserOrderDto[] GetUserOrders(string userId)
        {
            string userOrdersCacheKey = string.Format(USER_ORDER_CACHE_KEY_TEMPLATE, FAKE_USER_ID);

            UserOrderDto[] userOrderDtos = base.cacheAdapter.Get<UserOrderDto[]>(userOrdersCacheKey);
            if (userOrderDtos is null)
            {
                Order[] userOrders = base.repository.GetUserOrders(userId);
                userOrderDtos = base.mapper.Map<Order[], UserOrderDto[]>(userOrders);

                base.cacheAdapter.Set(userOrdersCacheKey, userOrderDtos);
            }

            return userOrderDtos;
        }

        public void Create(CreateOrderDto orderDto)
        {
            base.cacheAdapter.Remove(PENDING_ORDERS_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(USER_ORDER_CACHE_KEY_TEMPLATE, FAKE_USER_ID));

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

            base.cacheAdapter.Remove(ItemService.MARKETPLACE_CACHE_KEY);
            base.cacheAdapter.Remove(ItemService.INVENTORY_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(ItemService.ITEM_CACHE_KEY_TEMPLATE, order.ItemId));
            base.cacheAdapter.Remove(PENDING_ORDERS_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(USER_ORDER_CACHE_KEY_TEMPLATE, FAKE_USER_ID));

            this.itemRepository.BuyItem(order.ItemId, order.Quantity);

            base.repository.Finish(id);
        }

        public void Decline(int id)
        {
            base.cacheAdapter.Remove(PENDING_ORDERS_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(USER_ORDER_CACHE_KEY_TEMPLATE, FAKE_USER_ID));

            base.repository.Delete(id);
        }
    }
}