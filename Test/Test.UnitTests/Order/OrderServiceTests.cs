namespace Test.UnitTests.Order
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
            orderRepository = new Mock<IOrderRepository>();
            itemRepository = new Mock<IItemRepository>();
            memoryCache = new Mock<IMemoryCacheAdapter>();
            mapper = new Mock<IMapper>();
            httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContext = new Mock<HttpContext>();
            orderService = new OrderService(orderRepository.Object, itemRepository.Object, memoryCache.Object,
                                            mapper.Object, httpContextAccessor.Object);
            pendingOrder = new Order()
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

            finishedOrder = new Order()
            {
                Id = FINISHED_ORDER_ID,
                ItemId = ITEM_ID,
                ItemCode = ITEM_CODE,
                Quantity = ORDER_ITEM_QUANTITY,
                Email = VSG_EMAIL,
                Status = OrderStatus.Finished,
                CreatedAtUtc = DateTime.UtcNow,
            };

            orderItem = new Item()
            {
                Id = ITEM_ID,
                Code = ITEM_CODE,
                Name = ITEM_NAME,
                Price = ITEM_PRICE,
                Category = Category.Laptops,
                QuantityCombined = ITEM_QUANTITY_COMBINED,
                QuantityForSale = ITEM_QUANTITY_FOR_SALE
            };

            pendingOrderDtos = new PendingOrderDto[]
            {
                new PendingOrderDto()
                {
                    ItemCode = ITEM_CODE,
                    Quantity = ORDER_ITEM_QUANTITY,
                    Price = ITEM_PRICE,
                    OrderDate = pendingOrder.CreatedAtUtc.ToLocalTime()
                }
            };
            mapper.Setup(m => m.Map<Order[], PendingOrderDto[]>(It.IsAny<Order[]>())).Returns(pendingOrderDtos);

            UserOrderDto userPendingOrder = new UserOrderDto()
            {
                ItemName = ITEM_NAME,
                Quantity = ORDER_ITEM_QUANTITY,
                Price = ITEM_PRICE,
                OrderDate = pendingOrder.CreatedAtUtc.ToLocalTime(),
                Status = OrderStatus.Pending.ToString()
            };
            UserOrderDto userFinishedOrder = new UserOrderDto()
            {
                ItemName = ITEM_NAME,
                Quantity = ORDER_ITEM_QUANTITY,
                Price = ITEM_PRICE,
                OrderDate = pendingOrder.CreatedAtUtc.ToLocalTime(),
                Status = OrderStatus.Finished.ToString()
            };
            userOrderDtos = new UserOrderDto[]
            {
                new UserOrderDto()
                {
                    ItemName = ITEM_NAME,
                    Quantity = ORDER_ITEM_QUANTITY,
                    Price = ITEM_PRICE,
                    OrderDate = pendingOrder.CreatedAtUtc.ToLocalTime(),
                    Status = OrderStatus.Pending.ToString()
                },
                new UserOrderDto()
                {
                    ItemName = ITEM_NAME,
                    Quantity = ORDER_ITEM_QUANTITY,
                    Price = ITEM_PRICE,
                    OrderDate = pendingOrder.CreatedAtUtc.ToLocalTime(),
                    Status = OrderStatus.Finished.ToString()
                }
            };

            List<Claim> claims = new List<Claim>()
            {
                new Claim(PREFERRED_USERNAME_CLAIM_NAME, VSG_EMAIL)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, AUTHENTICATION_TYPE_NAME);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            httpContext.Setup(c => c.User).Returns(principal);
            httpContextAccessor.Setup(c => c.HttpContext).Returns(httpContext.Object);

            mapper.Setup(m => m.Map<Order[], UserOrderDto[]>(It.IsAny<Order[]>())).Returns(userOrderDtos);

            createOrderDto = new CreateOrderDto()
            {
                ItemId = ITEM_ID,
                Quantity = ORDER_ITEM_QUANTITY
            };
            mapper.Setup(m => m.Map<CreateOrderDto, Order>(It.IsAny<CreateOrderDto>())).Returns(pendingOrder);

            Order[] pendingOrders = new Order[] { pendingOrder };
            Order[] orders = new Order[] { pendingOrder, finishedOrder };

            orderRepository.Setup(or => or.GetPendingOrders()).Returns(pendingOrders);
            orderRepository.Setup(or => or.GetUserOrders(VSG_EMAIL)).Returns(orders);
            orderRepository.Setup(or => or.GetOrderItemIdAndQuantity(PENDING_ORDER_ID)).Returns(pendingOrder);
        }

        [Test]
        public void GetPendingOrders_NotCached_ReturnsPendingOrderDtoArrayMappedFromRepository()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<PendingOrderDto[]>(It.IsAny<string>())).Returns((PendingOrderDto[])null);

            // Act
            PendingOrderDto[] pendingOrders = orderService.GetPendingOrders();

            // Assert
            pendingOrders.Should().BeEquivalentTo(pendingOrderDtos);
        }

        [Test]
        public void GetPendingOrders_Cached_ReturnsPendingOrderDtoArrayFromCache()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<PendingOrderDto[]>(It.IsAny<string>())).Returns(pendingOrderDtos);

            // Act
            PendingOrderDto[] pendingOrders = orderService.GetPendingOrders();

            // Assert
            pendingOrders.Should().BeEquivalentTo(pendingOrderDtos);
        }

        [Test]
        public void GetUserOrders_NotCached_ReturnsUserOrderDtoArrayMappedFromRepository()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<UserOrderDto[]>(It.IsAny<string>())).Returns((UserOrderDto[])null);

            // Act
            UserOrderDto[] userOrders = orderService.GetUserOrders();

            // Assert
            userOrders.Should().BeEquivalentTo(userOrderDtos);
        }

        [Test]
        public void GetUserOrders_Cached_ReturnsUserOrderDtoArrayFromCache()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<UserOrderDto[]>(It.IsAny<string>())).Returns(userOrderDtos);

            // Act
            UserOrderDto[] userOrders = orderService.GetUserOrders();

            // Assert
            userOrders.Should().BeEquivalentTo(userOrderDtos);
        }

        [Test]
        public void Create_NonExistingItem_ThrowsArgumentException()
        {
            // Arrange
            itemRepository.Setup(ir => ir.GetOrderItemInfoById(ITEM_ID)).Returns((Item)null);

            // Act
            Action action = () => orderService.Create(createOrderDto);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Create_WithInsufficientItemQuantityForSaleOrder_ThrowsArgumentException()
        {
            // Arrange
            orderItem.QuantityForSale = 0;
            itemRepository.Setup(ir => ir.GetOrderItemInfoById(ITEM_ID)).Returns(orderItem);

            // Act
            Action action = () => orderService.Create(createOrderDto);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Create_ValidOrder_DoesNotThrowException()
        {
            // Arrange
            orderItem.QuantityForSale = ORDER_ITEM_QUANTITY_COMBINED;
            itemRepository.Setup(ir => ir.GetOrderItemInfoById(ITEM_ID)).Returns(orderItem);

            // Act
            Action action = () => orderService.Create(createOrderDto);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void Finish_ItemNotFound_ThrowsNotFoundException()
        {
            // Arrange
            orderRepository.Setup(or => or.GetOrderItemIdAndQuantity(It.IsAny<string>())).Returns((Order)null);

            // Act
            Action action = () => orderService.Finish(PENDING_ORDER_ID);

            // Assert
            action.Should().Throw<NotFoundException>();
        }

        [Test]
        public void Finish_ValidOrderItem_DoesNotThrowException()
        {
            // Arrange
            orderItem.QuantityForSale = ITEM_QUANTITY_COMBINED;
            orderRepository.Setup(ir => ir.GetOrderItemIdAndQuantity(PENDING_ORDER_ID)).Returns(pendingOrder);

            // Act
            Action action = () => orderService.Finish(PENDING_ORDER_ID);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void Decline_ExistingOrder_DoesNotThrowException()
        {
            // Arrange
            orderItem.QuantityForSale = ITEM_QUANTITY_COMBINED;
            itemRepository.Setup(ir => ir.GetOrderItemInfoById(ITEM_ID)).Returns(orderItem);

            // Act
            Action action = () => orderService.Decline(PENDING_ORDER_ID);

            // Assert
            action.Should().NotThrow();
        }
    }
}