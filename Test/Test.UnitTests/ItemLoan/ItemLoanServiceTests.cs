namespace Test.UnitTests.ItemLoan
{
    using AutoMapper;
    using FluentAssertions;

    using Moq;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;
    using VSGBulgariaMarketplace.Application.Services;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    using static Constants.UserConstant;
    using static Constants.ItemLoanConstant;
    using static Constants.ItemConstant;

    [TestOf(typeof(ItemLoanService))]
    public class ItemLoanServiceTests
    {
        private readonly DateTime userLendItemStartDate = DateTime.UtcNow.Date;

        private readonly Mock<IItemRepository> itemRepository;
        private readonly Mock<IItemLoanRepository> itemLoanRepository;
        private readonly Mock<IMemoryCacheAdapter> memoryCache;
        private readonly Mock<IMapper> mapper;

        private readonly ItemLoanService itemLoanService;

        private readonly ItemLoan itemLoan;
        private readonly Dictionary<string, int> emailsWithLendItemsCount;
        private readonly List<EmailWithLendItemsCountDto> emailsWithLendItemsCountDto;
        private readonly UserLendItemDto[] userLendItemDtos;
        private readonly LendItemsDto lendsItemsDto;

        public ItemLoanServiceTests()
        {
            itemRepository = new Mock<IItemRepository>();
            itemLoanRepository = new Mock<IItemLoanRepository>();
            memoryCache = new Mock<IMemoryCacheAdapter>();
            mapper = new Mock<IMapper>();

            itemLoanService = new ItemLoanService(itemLoanRepository.Object, itemRepository.Object, memoryCache.Object,
                                                mapper.Object);

            emailsWithLendItemsCount = new Dictionary<string, int>()
            {
                { VSG_EMAIL, LEND_ITEMS_COUNT }
            };
            emailsWithLendItemsCountDto = new List<EmailWithLendItemsCountDto>()
            {
                new EmailWithLendItemsCountDto()
                {
                    Email = VSG_EMAIL,
                    LendItemsCount = LEND_ITEMS_COUNT
                }
            };
            mapper.Setup(m => m.Map<Dictionary<string, int>, List<EmailWithLendItemsCountDto>>(emailsWithLendItemsCount))
                        .Returns(emailsWithLendItemsCountDto);

            userLendItemDtos = new UserLendItemDto[]
            {
                new UserLendItemDto
                {
                    Id = LEND_ITEM_ID,
                    ItemId = ITEM_ID,
                    Email = VSG_EMAIL,
                    Quantity = LEND_ITEMS_COUNT,
                    StartDate = userLendItemStartDate,
                }
            };
            mapper.Setup(m => m.Map<ItemLoan[], UserLendItemDto[]>(It.IsAny<ItemLoan[]>())).Returns(userLendItemDtos);

            lendsItemsDto = new LendItemsDto()
            {
                Quantity = LEND_ITEMS_QUANTITY,
                Email = VSG_EMAIL
            };

            itemLoan = new ItemLoan()
            {
                ItemId = ITEM_ID,
                Email = VSG_EMAIL,
                Quantity = LEND_ITEMS_QUANTITY
            };
            mapper.Setup(m => m.Map<LendItemsDto, ItemLoan>(lendsItemsDto)).Returns(itemLoan);

            itemLoanRepository.Setup(ilr => ilr.GetUserEmailWithLendItemsCount()).Returns(emailsWithLendItemsCount);
        }

        [Test]
        public void GetUserEmailsWithLendItemsCount_NotCached_ReturnsEmailWithLendItemsCountDtoListMappedFromRepository()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<List<EmailWithLendItemsCountDto>>(It.IsAny<string>())).Returns((List<EmailWithLendItemsCountDto>)null);

            // Act
            List<EmailWithLendItemsCountDto> emailsWithLendItemsCountDto = itemLoanService.GetUserEmailsWithLendItemsCount();

            // Assert
            emailsWithLendItemsCountDto.Should().BeEquivalentTo(this.emailsWithLendItemsCountDto);
        }

        [Test]
        public void GetUserEmailsWithLendItemsCount_Cached_ReturnsEmailWithLendItemsCountDtoListFromCache()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<List<EmailWithLendItemsCountDto>>(It.IsAny<string>())).Returns(this.emailsWithLendItemsCountDto);

            // Act
            List<EmailWithLendItemsCountDto> emailsWithLendItemsCountDto = itemLoanService.GetUserEmailsWithLendItemsCount();

            // Assert
            emailsWithLendItemsCountDto.Should().BeEquivalentTo(this.emailsWithLendItemsCountDto);
        }


        [Test]
        public void GetUserLendItems_NotCached_ReturnsUserLendItemDtoArrayMappedFromRepository()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<UserLendItemDto[]>(It.IsAny<string>())).Returns((UserLendItemDto[])null);

            // Act
            UserLendItemDto[] userLendItemDtos = itemLoanService.GetUserLendItems(VSG_EMAIL);

            // Assert
            userLendItemDtos.Should().BeEquivalentTo(this.userLendItemDtos);
        }

        [Test]
        public void GetUserLendItems_Cached_ReturnsUserLendItemDtoArrayFromCache()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<UserLendItemDto[]>(It.IsAny<string>())).Returns(this.userLendItemDtos);

            // Act
            UserLendItemDto[] userLendItemDtos = itemLoanService.GetUserLendItems(VSG_EMAIL);

            // Assert
            userLendItemDtos.Should().BeEquivalentTo(this.userLendItemDtos);
        }

        [Test]
        public void GetUserLendItems_InvalidEmail_ThrowsArgumentException()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<UserLendItemDto[]>(It.IsAny<string>())).Returns(userLendItemDtos);

            // Act
            Action action = () => itemLoanService.GetUserLendItems(INVALID_EMAIL);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void LendItems_AvailableQuantityIsNull_ThrowsNotFoundException()
        {
            // Arrange
            itemRepository.Setup(ir => ir.TryGetAvailableQuantity(LEND_ITEM_ID, out It.Ref<int?>.IsAny)).Returns(false);

            // Act
            Action action = () => itemLoanService.LendItems(LEND_ITEM_ID, lendsItemsDto);

            // Assert
            action.Should().Throw<NotFoundException>();
        }

        [Test]
        public void LendItems_AvailableQuantityIsZero_ThrowsArgumentException()
        {
            // Arrange
            int? availableQuantity = 0;
            itemRepository.Setup(ir => ir.TryGetAvailableQuantity(LEND_ITEM_ID, out availableQuantity)).Returns(true);

            // Act
            Action action = () => itemLoanService.LendItems(LEND_ITEM_ID, lendsItemsDto);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void LendItems_ValidAvailableQuantity_DoesNotThrowException()
        {
            // Arrange
            int? availableQuantity = ITEM_AVAILABLE_QUANTITY;
            itemRepository.Setup(ir => ir.TryGetAvailableQuantity(LEND_ITEM_ID, out availableQuantity)).Returns(true);

            // Act
            Action action = () => itemLoanService.LendItems(LEND_ITEM_ID, lendsItemsDto);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void Return_NonExistingItemLoan_ThrowsNotFoundException()
        {
            // Arrange
            itemLoanRepository.Setup(ilr => ilr.GetItemLoanItemIdQuantityAndEmail(LEND_ITEM_ID)).Returns((ItemLoan)null);

            // Act
            Action action = () => itemLoanService.Return(LEND_ITEM_ID);

            // Assert
            action.Should().Throw<NotFoundException>();
        }

        [Test]
        public void Return_ExistingItemLoan_DoesNotThrowException()
        {
            // Arrange
            itemLoanRepository.Setup(ilr => ilr.GetItemLoanItemIdQuantityAndEmail(LEND_ITEM_ID)).Returns(itemLoan);

            // Act
            Action action = () => itemLoanService.Return(LEND_ITEM_ID);

            // Assert
            action.Should().NotThrow();
        }
    }
}