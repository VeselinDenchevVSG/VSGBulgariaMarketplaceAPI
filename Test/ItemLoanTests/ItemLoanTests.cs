namespace Test.ItemLoanTests
{
    using AutoMapper;
    using FluentAssertions;

    using Moq;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Image.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Services;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemLoanTests
    {
        private const string USER_EMAIL = "vdenchev@vsgbg.com";
        private const short LEND_ITEMS_COUNT = 1;
        private const string LEND_ITEM_ID = "Test";
        private const string LEND_ITEM_ITEM_ID = "Test";
        private const short LEND_ITEMS_QUANTITY = 1;
        private const short ITEM_AVAILABLE_QUANTITY = 1;

        private readonly string userLendItemStartDate = DateTime.UtcNow.Date.ToString();

        private readonly Mock<IItemRepository> itemRepository;
        private readonly Mock<IItemLoanRepository> itemLoanRepository;
        private readonly Mock<IOrderRepository> orderRepository;
        private readonly Mock<ICloudImageService> imageService;
        private readonly Mock<IMemoryCacheAdapter> memoryCache;
        private readonly Mock<IMapper> mapper;

        private readonly ItemLoanService itemLoanService;

        private readonly ItemLoan itemLoan;
        private readonly Dictionary<string, int> emailsWithLendItemsCount;
        private readonly List<EmailWithLendItemsCountDto> emailsWithLendItemsCountDto;
        private readonly UserLendItemDto[] userLendItemDtos;
        private readonly LendItemsDto lendsItemsDto;

        public ItemLoanTests()
        {
            this.itemRepository = new Mock<IItemRepository>();
            this.orderRepository = new Mock<IOrderRepository>();
            this.itemLoanRepository = new Mock<IItemLoanRepository>();
            this.imageService = new Mock<ICloudImageService>();
            this.memoryCache = new Mock<IMemoryCacheAdapter>();
            this.mapper = new Mock<IMapper>();

            this.itemLoanService = new ItemLoanService(this.itemLoanRepository.Object, this.itemRepository.Object, this.memoryCache.Object,
                                                this.mapper.Object);

            this.emailsWithLendItemsCount = new Dictionary<string, int>()
            {
                { USER_EMAIL, LEND_ITEMS_COUNT }
            };
            this.emailsWithLendItemsCountDto = new List<EmailWithLendItemsCountDto>()
            {
                new EmailWithLendItemsCountDto()
                {
                    Email = USER_EMAIL,
                    LendItemsCount = LEND_ITEMS_COUNT
                }
            };
            this.mapper.Setup(m => m.Map<Dictionary<string, int>, List<EmailWithLendItemsCountDto>> (this.emailsWithLendItemsCount))
                        .Returns(this.emailsWithLendItemsCountDto);

            this.userLendItemDtos = new UserLendItemDto[]
            {
                new UserLendItemDto
                {
                    Id = LEND_ITEM_ID,
                    ItemId = LEND_ITEM_ITEM_ID,
                    Email = USER_EMAIL,
                    Quantity = LEND_ITEMS_COUNT,
                    StartDate = userLendItemStartDate,
                }
            };
            this.mapper.Setup(m => m.Map<ItemLoan[], UserLendItemDto[]>(It.IsAny<ItemLoan[]>())).Returns(this.userLendItemDtos);

            this.lendsItemsDto = new LendItemsDto()
            {
                Quantity = LEND_ITEMS_QUANTITY,
                Email = USER_EMAIL
            };

            this.itemLoan = new ItemLoan()
            {
                ItemId = LEND_ITEM_ITEM_ID,
                Email = USER_EMAIL,
                Quantity = LEND_ITEMS_QUANTITY
            };

            this.itemLoanRepository.Setup(ilr => ilr.GetUserEmailWithLendItemsCount()).Returns(this.emailsWithLendItemsCount);
        }

        [Test]
        public void GetUserEmailsWithLendItemsCount_Should_Return_EmailWithLendItemsCountDtoList_Mapped_From_Repository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<List<EmailWithLendItemsCountDto>>(It.IsAny<string>())).Returns((List<EmailWithLendItemsCountDto>)null);

            // Act
            List<EmailWithLendItemsCountDto> emailsWithLendItemsCountDto = this.itemLoanService.GetUserEmailsWithLendItemsCount();

            // Assert
            emailsWithLendItemsCountDto.Should().BeEquivalentTo(this.emailsWithLendItemsCountDto);
        }

        [Test]
        public void GetUserEmailsWithLendItemsCount_Should_Return_EmailWithLendItemsCountDtoList_Mapped_From_Cache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<List<EmailWithLendItemsCountDto>>(It.IsAny<string>())).Returns(this.emailsWithLendItemsCountDto);

            // Act
            List<EmailWithLendItemsCountDto> emailsWithLendItemsCountDto = this.itemLoanService.GetUserEmailsWithLendItemsCount();

            // Assert
            emailsWithLendItemsCountDto.Should().BeEquivalentTo(this.emailsWithLendItemsCountDto);
        }


        [Test]
        public void GetUserLendItems_Should_Return_UserLendItemDtoArray_Mapped_From_Repository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<UserLendItemDto[]>(It.IsAny<string>())).Returns((UserLendItemDto[]) null);

            // Act
            UserLendItemDto[] userLendItemDtos = this.itemLoanService.GetUserLendItems(USER_EMAIL);

            // Assert
            userLendItemDtos.Should().BeEquivalentTo(this.userLendItemDtos);
        }

        [Test]
        public void GetUserLendItems_Should_Return_UserLendItemDtoArray_From_Cache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<UserLendItemDto[]>(It.IsAny<string>())).Returns(this.userLendItemDtos);

            // Act
            UserLendItemDto[] userLendItemDtos = this.itemLoanService.GetUserLendItems(USER_EMAIL);

            // Assert
            userLendItemDtos.Should().BeEquivalentTo(this.userLendItemDtos);
        }

        [Test]
        public void LendItems_Should_Throw_ArgumentException()
        {
            // Arrange
            this.itemRepository.Setup(ir => ir.TryGetAvailableQuantity(LEND_ITEM_ID, out It.Ref<int?>.IsAny)).Returns(false);

            // Act
            Action action = () => this.itemLoanService.LendItems(LEND_ITEM_ID, this.lendsItemsDto);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void LendItems_Should_Not_Throw_Exception()
        {
            // Arrange
            this.itemRepository.Setup(ir => ir.TryGetAvailableQuantity(LEND_ITEM_ID, out It.Ref<int?>.IsAny)).Returns(true);

            // Act
            Action action = () => this.itemLoanService.LendItems(LEND_ITEM_ID, this.lendsItemsDto);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void Return_Should_Throw_NotFoundException()
        {
            // Arrange
            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoanItemIdAndQuantity(LEND_ITEM_ID)).Returns((ItemLoan) null);

            // Act
            Action action = () => this.itemLoanService.Return(LEND_ITEM_ID);

            // Assert
            action.Should().Throw<NotFoundException>();
        }

        [Test]
        public void Return_Should_Not_Throw_Exception()
        {
            // Arrange
            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoanItemIdAndQuantity(LEND_ITEM_ID)).Returns(this.itemLoan);

            // Act
            Action action = () => this.itemLoanService.Return(LEND_ITEM_ID);

            // Assert
            action.Should().NotThrow();
        }
    }
}