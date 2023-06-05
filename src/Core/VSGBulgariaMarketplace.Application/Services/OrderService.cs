namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using FluentValidation;

    using Microsoft.AspNetCore.Http;
    using VSGBulgariaMarketplace.Application.Helpers.Validators.Order;
    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class OrderService : BaseService<IOrderRepository, Order>, IOrderService
    {
        private const string PENDING_ORDERS_CACHE_KEY = "pending-orders";
        private const string USER_ORDER_CACHE_KEY_TEMPLATE = "orders-user-{0}";

        private IItemRepository itemRepository;
        private IHttpContextAccessor httpContextAccessor;

        public OrderService(IOrderRepository repository, IItemRepository itemRepository, IMemoryCacheAdapter cacheAdapter, IMapper mapper, IHttpContextAccessor httpContextAccessor) 
            : base(repository, cacheAdapter, mapper)
        {
            this.itemRepository = itemRepository;
            this.httpContextAccessor = httpContextAccessor;
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

        public UserOrderDto[] GetUserOrders()
        {
            string email = this.httpContextAccessor.HttpContext.User.FindFirst("preferred_username").Value;

            string userOrdersCacheKey = string.Format(USER_ORDER_CACHE_KEY_TEMPLATE, email);

            UserOrderDto[] userOrderDtos = base.cacheAdapter.Get<UserOrderDto[]>(userOrdersCacheKey);
            if (userOrderDtos is null)
            {
                Order[] userOrders = base.repository.GetUserOrders(email);
                userOrderDtos = base.mapper.Map<Order[], UserOrderDto[]>(userOrders);

                base.cacheAdapter.Set(userOrdersCacheKey, userOrderDtos);
            }

            return userOrderDtos;
        }

        public void Create(CreateOrderDto orderDto)
        {
            string email = this.httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "preferred_username").Value;

            CreateOrderDtoValidator validator = new CreateOrderDtoValidator();
            validator.ValidateAndThrow(orderDto);

            Order order = base.mapper.Map<CreateOrderDto, Order>(orderDto);

            Item item = this.itemRepository.GetOrderItemInfoById(orderDto.ItemId);
            if (item is null) throw new ArgumentException($"Item doesn't exist!");
            else
            {
                bool isEnoughQuantity = orderDto.Quantity <= item.QuantityForSale;
                if (isEnoughQuantity)
                {
                    order.ItemId = item.Id;
                    order.ItemCode = item.Code;
                    order.ItemName = item.Name;
                    order.ItemPrice = item.Price;
                    order.Email = email;

                    base.repository.Create(order);
                    this.itemRepository.RequestItemPurchase(order.ItemId, order.Quantity);

                    this.ClearOrderItemRelatedCache(order.ItemId, email);
                }
                else throw new ArgumentException("Not enough item quantity for sale!");
            }
        }

        public void Finish(string id)
        {
            Order order = base.repository.GetOrderItemIdAndQuantity(id);
            if (order is null) throw new NotFoundException($"Order doesn't exist!");

            string email = this.httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "preferred_username").Value;

            this.itemRepository.BuyItem(order.ItemId, order.Quantity);
            base.repository.Finish(id);

            this.ClearOrderItemRelatedCache(order.ItemId, email);
        }

        public void Decline(string id)
        {
            Order order = base.repository.GetOrderItemIdAndQuantity(id);

            string email = this.httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == "preferred_username").Value;

            base.repository.Delete(id);
            this.itemRepository.RestoreItemQuantitiesWhenOrderIsDeclined(order.ItemId, order.Quantity);

            this.ClearOrderItemRelatedCache(order.ItemId, email);
        }

        public void ClearOrderItemRelatedCache(string itemId, string email)
        {
            base.cacheAdapter.Remove(ItemService.MARKETPLACE_CACHE_KEY);
            base.cacheAdapter.Remove(ItemService.INVENTORY_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(ItemService.ITEM_CACHE_KEY_TEMPLATE, itemId));
            base.cacheAdapter.Remove(PENDING_ORDERS_CACHE_KEY);
            base.cacheAdapter.Remove(string.Format(USER_ORDER_CACHE_KEY_TEMPLATE, email));
        }
    }
}