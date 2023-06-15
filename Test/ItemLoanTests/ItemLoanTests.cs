namespace Test.ItemLoanTests
{
    using AutoMapper;
    using FluentAssertions;

    using Moq;

    using Test.Constants;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;
    using VSGBulgariaMarketplace.Application.Services;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemLoanTests
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

        public ItemLoanTests()
        {
            this.itemRepository = new Mock<IItemRepository>();
            this.itemLoanRepository = new Mock<IItemLoanRepository>();
            this.memoryCache = new Mock<IMemoryCacheAdapter>();
            this.mapper = new Mock<IMapper>();

            this.itemLoanService = new ItemLoanService(this.itemLoanRepository.Object, this.itemRepository.Object, this.memoryCache.Object,
                                                this.mapper.Object);

            this.emailsWithLendItemsCount = new Dictionary<string, int>()
            {
                { UserConstant.USER_EMAIL, ItemLoanConstant.LEND_ITEMS_COUNT }
            };
            this.emailsWithLendItemsCountDto = new List<EmailWithLendItemsCountDto>()
            {
                new EmailWithLendItemsCountDto()
                {
                    Email = UserConstant.USER_EMAIL,
                    LendItemsCount = ItemLoanConstant.LEND_ITEMS_COUNT
                }
            };
            this.mapper.Setup(m => m.Map<Dictionary<string, int>, List<EmailWithLendItemsCountDto>> (this.emailsWithLendItemsCount))
                        .Returns(this.emailsWithLendItemsCountDto);

            this.userLendItemDtos = new UserLendItemDto[]
            {
                new UserLendItemDto
                {
                    Id = ItemLoanConstant.LEND_ITEM_ID,
                    ItemId = ItemConstant.ITEM_ID,
                    Email = UserConstant.USER_EMAIL,
                    Quantity = ItemLoanConstant.LEND_ITEMS_COUNT,
                    StartDate = userLendItemStartDate,
                }
            };
            this.mapper.Setup(m => m.Map<ItemLoan[], UserLendItemDto[]>(It.IsAny<ItemLoan[]>())).Returns(this.userLendItemDtos);

            this.lendsItemsDto = new LendItemsDto()
            {
                Quantity = ItemLoanConstant.LEND_ITEMS_QUANTITY,
                Email = UserConstant.USER_EMAIL
            };

            this.itemLoan = new ItemLoan()
            {
                ItemId = ItemConstant.ITEM_ID,
                Email = UserConstant.USER_EMAIL,
                Quantity = ItemLoanConstant.LEND_ITEMS_QUANTITY
            };
            this.mapper.Setup(m => m.Map<LendItemsDto, ItemLoan>(this.lendsItemsDto)).Returns(this.itemLoan);

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
            UserLendItemDto[] userLendItemDtos = this.itemLoanService.GetUserLendItems(UserConstant.USER_EMAIL);

            // Assert
            userLendItemDtos.Should().BeEquivalentTo(this.userLendItemDtos);
        }

        [Test]
        public void GetUserLendItems_Should_Return_UserLendItemDtoArray_From_Cache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<UserLendItemDto[]>(It.IsAny<string>())).Returns(this.userLendItemDtos);

            // Act
            UserLendItemDto[] userLendItemDtos = this.itemLoanService.GetUserLendItems(UserConstant.USER_EMAIL);

            // Assert
            userLendItemDtos.Should().BeEquivalentTo(this.userLendItemDtos);
        }

        [Test]
        public void GetUserLendItems_Should_Throw_ArgumentException()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<UserLendItemDto[]>(It.IsAny<string>())).Returns(this.userLendItemDtos);

            // Act
            Action action = () => this.itemLoanService.GetUserLendItems(null);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void LendItems_Should_Throw_NotFoundException()
        {
            // Arrange
            this.itemRepository.Setup(ir => ir.TryGetAvailableQuantity(ItemLoanConstant.LEND_ITEM_ID, out It.Ref<int?>.IsAny)).Returns(false);

            // Act
            Action action = () => this.itemLoanService.LendItems(ItemLoanConstant.LEND_ITEM_ID, this.lendsItemsDto);

            // Assert
            action.Should().Throw<NotFoundException>();
        }

        [Test]
        public void LendItems_Should_Throw_ArgumentException()
        {
            // Arrange
            int? availableQuantity = 0;
            this.itemRepository.Setup(ir => ir.TryGetAvailableQuantity(ItemLoanConstant.LEND_ITEM_ID, out availableQuantity)).Returns(true);

            // Act
            Action action = () => this.itemLoanService.LendItems(ItemLoanConstant.LEND_ITEM_ID, this.lendsItemsDto);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void LendItems_Should_Not_Throw_Exception()
        {
            // Arrange
            int? availableQuantity = ItemConstant.ITEM_AVAILABLE_QUANTITY;
            this.itemRepository.Setup(ir => ir.TryGetAvailableQuantity(ItemLoanConstant.LEND_ITEM_ID, out availableQuantity)).Returns(true);

            // Act
            Action action = () => this.itemLoanService.LendItems(ItemLoanConstant.LEND_ITEM_ID, this.lendsItemsDto);

            // Assert
            action.Should().NotThrow();
        }

        [Test]
        public void Return_Should_Throw_NotFoundException()
        {
            // Arrange
            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoanItemIdQuantityAndEmail(ItemLoanConstant.LEND_ITEM_ID)).Returns((ItemLoan) null);

            // Act
            Action action = () => this.itemLoanService.Return(ItemLoanConstant.LEND_ITEM_ID);

            // Assert
            action.Should().Throw<NotFoundException>();
        }

        [Test]
        public void Return_Should_Not_Throw_Exception()
        {
            // Arrange
            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoanItemIdQuantityAndEmail(ItemLoanConstant.LEND_ITEM_ID)).Returns(this.itemLoan);

            // Act
            Action action = () => this.itemLoanService.Return(ItemLoanConstant.LEND_ITEM_ID);

            // Assert
            action.Should().NotThrow();
        }
    }
}