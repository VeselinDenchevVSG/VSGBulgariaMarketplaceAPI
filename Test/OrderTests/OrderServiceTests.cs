namespace Test.OrderTests
{
    using AutoMapper;
    using FluentAssertions;

    using Microsoft.AspNetCore.Http;

    using Moq;

    using System.Security.Claims;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Services;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Domain.Enums;

    using static Constants.OrderConstant;
    using static Constants.ItemConstant;
    using static Constants.UserConstant;
    using static VSGBulgariaMarketplace.Application.Constants.AuthorizationConstant;

    [TestOf(typeof(OrderService))]
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> orderRepository;
        private readonly Mock<IItemRepository> itemRepository;
        private readonly Mock<IMemoryCacheAdapter> memoryCache;
        private readonly Mock<IMapper> mapper;
        private readonly Mock<IHttpContextAccessor> httpContextAccessor;
        private readonly Mock<HttpContext> httpContext;

        private readonly OrderService orderService;

        private readonly PendingOrderDto[] pendingOrderDtos;
        private readonly UserOrderDto[] userOrderDtos;
        private readonly CreateOrderDto createOrderDto;

        private readonly Order pendingOrder;
        private readonly Order finishedOrder;
        private readonly Item orderItem;

        public OrderServiceTests()
        {
            this.orderRepository = new Mock<IOrderRepository>();
            this.itemRepository = new Mock<IItemRepository>();
            this.memoryCache = new Mock<IMemoryCacheAdapter>();
            this.mapper = new Mock<IMapper>();
            this.httpContextAccessor = new Mock<IHttpContextAccessor>();
            this.httpContext = new Mock<HttpContext>();
            this.orderService = new OrderService(this.orderRepository.Object, this.itemRepository.Object, this.memoryCache.Object, 
                                            this.mapper.Object, this.httpContextAccessor.Object);
            this.pendingOrder = new Order()
            {
                Id = PENDING_ORDER_ID,
                ItemId = ITEM_ID,
                ItemCode = ITEM_CODE,
                ItemName = ITEM_NAME,
                ItemPrice = ITEM_PRICE,
                Quantity = ORDER_ITEM_QUANTITY,
                Email = VSG_EMAIL,
                Status = OrderStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow,
            };

            this.finishedOrder = new Order()
            {
                Id = FINISHED_ORDER_ID,
                ItemId = ITEM_ID,
                ItemCode = ITEM_CODE,
                Quantity = ORDER_ITEM_QUANTITY,
                Email = VSG_EMAIL,
                Status = OrderStatus.Finished,
                CreatedAtUtc = DateTime.UtcNow,
            };

            this.orderItem = new Item()
            {
                Id = ITEM_ID,
                Code = ITEM_CODE,
                Name = ITEM_NAME,
                Price = ITEM_PRICE,
                Category = Category.Laptops,
                QuantityCombined = ITEM_QUANTITY_COMBINED,
                QuantityForSale = ITEM_QUANTITY_FOR_SALE
            };

            this.pendingOrderDtos = new PendingOrderDto[] 
            {
                new PendingOrderDto()
                {
                    ItemCode = ITEM_CODE,
                    Quantity = ORDER_ITEM_QUANTITY,
                    Price = ITEM_PRICE,
                    OrderDate = this.pendingOrder.CreatedAtUtc.ToLocalTime()
                }
            };
            this.mapper.Setup(m => m.Map<Order[], PendingOrderDto[]>(It.IsAny<Order[]>())).Returns(this.pendingOrderDtos);

            UserOrderDto userPendingOrder = new UserOrderDto()
            {
                ItemName = ITEM_NAME,
                Quantity = ORDER_ITEM_QUANTITY,
                Price = ITEM_PRICE,
                OrderDate = this.pendingOrder.CreatedAtUtc.ToLocalTime(),
                Status = OrderStatus.Pending.ToString()
            };
            UserOrderDto userFinishedOrder = new UserOrderDto()
            {
                ItemName = ITEM_NAME,
                Quantity = ORDER_ITEM_QUANTITY,
                Price = ITEM_PRICE,
                OrderDate = this.pendingOrder.CreatedAtUtc.ToLocalTime(),
                Status = OrderStatus.Finished.ToString()
            };
            this.userOrderDtos = new UserOrderDto[] 
            {
                new UserOrderDto()
                {
                    ItemName = ITEM_NAME,
                    Quantity = ORDER_ITEM_QUANTITY,
                    Price = ITEM_PRICE,
                    OrderDate = this.pendingOrder.CreatedAtUtc.ToLocalTime(),
                    Status = OrderStatus.Pending.ToString()
                },
                new UserOrderDto()
                {
                    ItemName = ITEM_NAME,
                    Quantity = ORDER_ITEM_QUANTITY,
                    Price = ITEM_PRICE,
                    OrderDate = this.pendingOrder.CreatedAtUtc.ToLocalTime(),
                    Status = OrderStatus.Finished.ToString()
                }
            };

            List<Claim> claims = new List<Claim>()
            {
                new Claim(PREFERRED_USERNAME_CLAIM_NAME, VSG_EMAIL)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, AUTHENTICATION_TYPE_NAME);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            this.httpContext.Setup(c => c.User).Returns(principal);
            this.httpContextAccessor.Setup(c => c.HttpContext).Returns(this.httpContext.Object);

            this.mapper.Setup(m => m.Map<Order[], UserOrderDto[]>(It.IsAny<Order[]>())).Returns(this.userOrderDtos);

            this.createOrderDto = new CreateOrderDto()
            {
                ItemId = ITEM_ID,
                Quantity = ORDER_ITEM_QUANTITY
            };
            this.mapper.Setup(m => m.Map<CreateOrderDto, Order>(It.IsAny<CreateOrderDto>())).Returns(this.pendingOrder);

            Order[] pendingOrders = new Order[] { this.pendingOrder };
            Order[] orders = new Order[] { this.pendingOrder, finishedOrder };

            this.orderRepository.Setup(or => or.GetPendingOrders()).Returns(pendingOrders);
            this.orderRepository.Setup(or => or.GetUserOrders(VSG_EMAIL)).Returns(orders);
            this.orderRepository.Setup(or => or.GetOrderItemIdAndQuantity(PENDING_ORDER_ID)).Returns(this.pendingOrder);
        }

        [Test]
        public void GetPendingOrders_NotCached_ReturnsPendingOrderDtoArrayMappedFromRepository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<PendingOrderDto[]>(It.IsAny<string>())).Returns((PendingOrderDto[])null);

            // Act
            PendingOrderDto[] pendingOrders = this.orderService.GetPendingOrders();

            // Assert
            pendingOrders.Should().BeEquivalentTo(this.pendingOrderDtos);
        }

        [Test]
        public void GetPendingOrders_Cached_ReturnsPendingOrderDtoArrayFromCache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<PendingOrderDto[]>(It.IsAny<string>())).Returns(this.pendingOrderDtos);

            // Act
            PendingOrderDto[] pendingOrders = this.orderService.GetPendingOrders();

            // Assert
            pendingOrders.Should().BeEquivalentTo(this.pendingOrderDtos);
        }

        [Test]
        public void GetUserOrders_NotCached_ReturnsUserOrderDtoArrayMappedFromRepository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<UserOrderDto[]>(It.IsAny<string>())).Returns((UserOrderDto[])null);

            // Act
            UserOrderDto[] userOrders = this.orderService.GetUserOrders();

            // Assert
            userOrders.Should().BeEquivalentTo(this.userOrderDtos);
        }

        [Test]
        public void GetUserOrders_Cached_ReturnsUserOrderDtoArrayFromCache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<UserOrderDto[]>(It.IsAny<string>())).Returns(this.userOrderDtos);

            // Act
            UserOrderDto[] userOrders = this.orderService.GetUserOrders();

            // Assert
            userOrders.Should().BeEquivalentTo(this.userOrderDtos);
        }

        [Test]
        public void Create_NonExistingItem_ThrowsArgumentException()
        {
            // Arrange
            this.itemRepository.Setup(ir => ir.GetOrderItemInfoById(ITEM_ID)).Returns((Item)null);

            // Act
            Action action = () => this.orderService.Create(this.createOrderDto);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Create_WithInsufficientItemQuantityForSaleOrder_ThrowsArgumentException()
        {
            // Arrange
            this.orderItem.QuantityForSale = 0;
            this.itemRepository.Setup(ir => ir.GetOrderItemInfoById(ITEM_ID)).Returns(this.orderItem);

            // Act
            Action action = () => this.orderService.Create(this.createOrderDto);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Create_ValidOrder_DoesNotThrowException()
        {
            // Arrange
            this.orderItem.QuantityForSale = ORDER_ITEM_QUANTITY_COMBINED;
            this.itemRepository.Setup(ir => ir.GetOrderItemInfoById(ITEM_ID)).Returns(this.orderItem);

            // Act
            Action action = () => this.orderService.Create(this.createOrderDto);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void Finish_ItemNotFound_ThrowsNotFoundException()
        {
            // Arrange
            this.orderRepository.Setup(or => or.GetOrderItemIdAndQuantity(It.IsAny<string>())).Returns((Order) null);

            // Act
            Action action = () => this.orderService.Finish(PENDING_ORDER_ID);

            // Assert
            action.Should().Throw<NotFoundException>();
        }

        [Test]
        public void Finish_ValidOrderItem_DoesNotThrowException()
        {
            // Arrange
            this.orderItem.QuantityForSale = ITEM_QUANTITY_COMBINED;
            this.orderRepository.Setup(ir => ir.GetOrderItemIdAndQuantity(PENDING_ORDER_ID)).Returns(this.pendingOrder);

            // Act
            Action action = () => this.orderService.Finish(PENDING_ORDER_ID);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void Decline_ExistingOrder_DoesNotThrowException()
        {
            // Arrange
            this.orderItem.QuantityForSale = ITEM_QUANTITY_COMBINED;
            itemRepository.Setup(ir => ir.GetOrderItemInfoById(ITEM_ID)).Returns(this.orderItem);

            // Act
            Action action = () => this.orderService.Decline(PENDING_ORDER_ID);

            // Assert
            action.Should().NotThrow();
        }
    }
}