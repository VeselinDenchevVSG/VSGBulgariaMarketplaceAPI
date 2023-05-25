namespace Test
{
    using AutoMapper;
    using FluentAssertions;

    using Microsoft.AspNetCore.Http;

    using Moq;

    using System.Security.Claims;

    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Services;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Domain.Enums;

    public class OrderServiceTests
    {
        private const int PENDING_ORDER_ID = 1;
        private const int FINISHED_ORDER_ID = 2;
        private const int ORDER_ITEM_CODE = 1;
        private const short ORDER_ITEM_QUANTITY = 1;
        private const decimal ORDER_ITEM_PRICE = 1.11m;
        private const string ORDER_ITEM_NAME = "Test";
        private const string ORDER_ITEM_IMAGE_PUBLIC_ID = "Test";
        private const string ORDER_ITEM_IMAGE_URL = "https://shorturl.at/fgwFK";
        private const short ORDER_ITEM_QUANTITY_COMBINED = 1;

        private const string USER_EMAIL = "vdenchev@vsgbg.com";

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

        public OrderServiceTests()
        {
            this.orderRepository = new Mock<IOrderRepository>();
            this.itemRepository = new Mock<IItemRepository>();
            this.memoryCache = new Mock<IMemoryCacheAdapter>();
            this.mapper = new Mock<IMapper>();
            this.httpContextAccessor = new Mock<IHttpContextAccessor>();
            this.httpContext = new Mock<HttpContext>();
            this.orderService = new OrderService(this.orderRepository.Object, this.itemRepository.Object, this.memoryCache.Object, this.mapper.Object, 
                                                    this.httpContextAccessor.Object);
            this.pendingOrder = new Order()
            {
                Id = PENDING_ORDER_ID,
                ItemId = ORDER_ITEM_CODE,
                Item = new Item()
                {
                    Id = ORDER_ITEM_CODE,
                    Name = ORDER_ITEM_NAME,
                    Image = new CloudinaryImage()
                    {
                        Id = ORDER_ITEM_IMAGE_PUBLIC_ID,
                        SecureUrl = ORDER_ITEM_IMAGE_URL
                    },
                    ImagePublicId = ORDER_ITEM_IMAGE_PUBLIC_ID,
                    Price = ORDER_ITEM_PRICE,
                    Category = Category.Laptops,
                    QuantityCombined = ORDER_ITEM_QUANTITY_COMBINED
                },
                Quantity = ORDER_ITEM_QUANTITY,
                Email = USER_EMAIL,
                Status = OrderStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow,
            };

            this.finishedOrder = new Order()
            {
                Id = FINISHED_ORDER_ID,
                ItemId = ORDER_ITEM_CODE,
                Quantity = ORDER_ITEM_QUANTITY,
                Email = USER_EMAIL,
                Status = OrderStatus.Finished,
                CreatedAtUtc = DateTime.UtcNow,
            };

            PendingOrderDto pendingOrder = new PendingOrderDto()
            {
                ItemCode = ORDER_ITEM_CODE,
                Quantity = ORDER_ITEM_QUANTITY,
                Price = ORDER_ITEM_PRICE,
                OrderDate = this.pendingOrder.CreatedAtUtc.ToLocalTime()
            };
            this.pendingOrderDtos = new PendingOrderDto[] { pendingOrder };
            this.mapper.Setup(m => m.Map<Order[], PendingOrderDto[]>(It.IsAny<Order[]>())).Returns(this.pendingOrderDtos);

            UserOrderDto userPendingOrder = new UserOrderDto()
            {
                ItemName = ORDER_ITEM_NAME,
                Quantity = ORDER_ITEM_QUANTITY,
                Price = ORDER_ITEM_PRICE,
                OrderDate = this.pendingOrder.CreatedAtUtc.ToLocalTime(),
                Status = OrderStatus.Pending.ToString()
            };
            UserOrderDto userFinishedOrder = new UserOrderDto()
            {
                ItemName = ORDER_ITEM_NAME,
                Quantity = ORDER_ITEM_QUANTITY,
                Price = ORDER_ITEM_PRICE,
                OrderDate = this.pendingOrder.CreatedAtUtc.ToLocalTime(),
                Status = OrderStatus.Finished.ToString()
            };
            UserOrderDto[] userOrders = new UserOrderDto[] { userPendingOrder, userFinishedOrder };

            List<Claim> claims = new List<Claim>()
            {
                new Claim("preferred_username", USER_EMAIL)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuthType");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            this.httpContext.Setup(c => c.User).Returns(principal);
            this.httpContextAccessor.Setup(c => c.HttpContext).Returns(this.httpContext.Object);

            this.mapper.Setup(m => m.Map<Order[], UserOrderDto[]>(It.IsAny<Order[]>())).Returns(this.userOrderDtos);

            this.createOrderDto = new CreateOrderDto()
            {
                ItemCode = ORDER_ITEM_CODE,
                Quantity = ORDER_ITEM_QUANTITY
            };
            this.mapper.Setup(m => m.Map<CreateOrderDto, Order>(It.IsAny<CreateOrderDto>())).Returns(this.pendingOrder);

            Order[] pendingOrders = new Order[] { this.pendingOrder };
            Order[] orders = new Order[] { this.pendingOrder, this.finishedOrder};

            this.orderRepository.Setup(or => or.GetPendingOrders()).Returns(pendingOrders);
            this.orderRepository.Setup(or => or.GetUserOrders(USER_EMAIL)).Returns(orders);
            this.orderRepository.Setup(or => or.GetOrderItemIdAndQuantity(PENDING_ORDER_ID)).Returns(this.pendingOrder);
            this.orderRepository.Setup(or => or.Finish(PENDING_ORDER_ID));
            this.orderRepository.Setup(or => or.Delete(PENDING_ORDER_ID));

            this.memoryCache.Setup(mc => mc.Set(It.IsAny<string>(), It.IsAny<object>()));
            this.memoryCache.Setup(mc => mc.Remove(It.IsAny<string>()));
        }

        [Test]
        public void GetPendingOrders_Should_Return_PendingOrderDtoArray_Mapped_From_Repository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<PendingOrderDto[]>(It.IsAny<string>())).Returns((PendingOrderDto[]) null);

            // Act
            PendingOrderDto[] pendingOrders = this.orderService.GetPendingOrders();

            // Assert
            pendingOrders.Should().BeEquivalentTo(this.pendingOrderDtos);
        }

        [Test]
        public void GetPendingOrders_Should_Return_PendingOrderDtoArray_Mapped_From_Cache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<PendingOrderDto[]>(It.IsAny<string>())).Returns(this.pendingOrderDtos);

            // Act
            PendingOrderDto[] pendingOrders = this.orderService.GetPendingOrders();

            // Assert
            pendingOrders.Should().BeEquivalentTo(this.pendingOrderDtos);
        }

        [Test]
        public void GetUserOrders_Should_Return_UserOrderDtoArray_Mapped_From_Repository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<UserOrderDto[]>(It.IsAny<string>())).Returns((UserOrderDto[]) null);

            // Act
            UserOrderDto[] userOrders = this.orderService.GetUserOrders();

            // Assert
            userOrders.Should().BeEquivalentTo(this.userOrderDtos);
        }

        [Test]
        public void GetUserOrders_Should_Return_UserOrderDtoArray_Mapped_From_Cache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<UserOrderDto[]>(It.IsAny<string>())).Returns(this.userOrderDtos);

            // Act
            UserOrderDto[] userOrders = this.orderService.GetUserOrders();

            // Assert
            userOrders.Should().BeEquivalentTo(this.userOrderDtos);
        }

        [Test]
        public void Create_Should_Throw_ArgumentException()
        {
            // Arrange
            this.pendingOrder.Item.QuantityForSale = ORDER_ITEM_QUANTITY_COMBINED;
            this.itemRepository.Setup(ir => ir.GetQuantityForSaleAndPriceByCode(ORDER_ITEM_CODE)).Returns((Item) null);

            // Act
            Action action = () => this.orderService.Create(this.createOrderDto);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Create_Should_Throw_ArgumentOutOfRangeException()
        {
            // Arrange
            this.pendingOrder.Item.QuantityForSale = 0;
            this.itemRepository.Setup(ir => ir.GetQuantityForSaleAndPriceByCode(ORDER_ITEM_CODE)).Returns(this.pendingOrder.Item);

            // Act
            Action action = () => this.orderService.Create(this.createOrderDto);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Create_Should_Not_Throw_Exception()
        {
            // Arrange
            this.pendingOrder.Item.QuantityForSale = ORDER_ITEM_QUANTITY_COMBINED;
            this.itemRepository.Setup(ir => ir.GetQuantityForSaleAndPriceByCode(ORDER_ITEM_CODE)).Returns(this.pendingOrder.Item);

            // Act
            Action action = () => this.orderService.Create(this.createOrderDto);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void Finish_Order_Should_Not_Throw_Exception()
        {
            // Arrange
            this.pendingOrder.Item.QuantityForSale = ORDER_ITEM_QUANTITY_COMBINED;
            this.itemRepository.Setup(ir => ir.GetQuantityForSaleAndPriceByCode(ORDER_ITEM_CODE)).Returns(this.pendingOrder.Item);

            // Act
            Action action = () => this.orderService.Finish(ORDER_ITEM_CODE);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void Decline_Order_Should_Not_Throw_Exception()
        {
            // Arrange
            this.pendingOrder.Item.QuantityForSale = ORDER_ITEM_QUANTITY_COMBINED;
            this.itemRepository.Setup(ir => ir.GetQuantityForSaleAndPriceByCode(ORDER_ITEM_CODE)).Returns(this.pendingOrder.Item);

            // Act
            Action action = () => this.orderService.Decline(ORDER_ITEM_CODE);

            // Assert
            action.Should().NotThrow();
        }
    }
}