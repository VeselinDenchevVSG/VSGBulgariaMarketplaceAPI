namespace Test.ItemTests
{
    using AutoMapper;
    using FluentAssertions;

    using Microsoft.AspNetCore.Http;

    using Moq;

    using VSGBulgariaMarketplace.Application.Models.Image.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Domain.Enums;
    using VSGBulgariaMarketplace.Application.Services;
    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;

    using static Constants.ItemConstant;

    [TestOf(typeof(ItemService))]
    public class ItemServiceTests
    {
        private readonly Mock<IItemRepository> itemRepository;
        private readonly Mock<IItemLoanRepository> itemLoanRepository;
        private readonly Mock<IOrderRepository> orderRepository;
        private readonly Mock<ICloudImageService> imageService;
        private readonly Mock<IMemoryCacheAdapter> memoryCache;
        private readonly Mock<IMapper> mapper;

        private readonly ItemService itemService;

        private readonly Item item;
        private readonly MarketplaceItemDto[] marketplace;
        private readonly InventoryItemDto[] inventory;
        private readonly ItemDetailsDto itemDetailsDto;
        private readonly CreateItemDto createItemDto;
        private readonly UpdateItemDto updateItemDto;

        public ItemServiceTests()
        {
            this.itemRepository = new Mock<IItemRepository>();
            this.orderRepository = new Mock<IOrderRepository>();
            this.itemLoanRepository = new Mock<IItemLoanRepository>();
            this.imageService = new Mock<ICloudImageService>();
            this.memoryCache = new Mock<IMemoryCacheAdapter>();
            this.mapper = new Mock<IMapper>();
            this.itemService = new ItemService(this.itemRepository.Object, this.orderRepository.Object, this.itemLoanRepository.Object, this.imageService.Object, 
                                                this.memoryCache.Object, this.mapper.Object);
            this.item = new Item()
            {
                Id = ITEM_ID,
                Code = ITEM_CODE,
                Name = ITEM_NAME,
                ImagePublicId = ITEM_IMAGE_PUBLIC_ID,
                Price = ITEM_PRICE,
                Category = Category.Laptops,
                QuantityCombined = ITEM_QUANTITY_COMBINED,
                AvailableQuantity = ITEM_AVAILABLE_QUANTITY
            };

            this.marketplace = new MarketplaceItemDto[] 
            {
                new MarketplaceItemDto()
                {
                    Code = ITEM_CODE,
                    ImageUrl = ITEM_IMAGE_URL,
                    Price = ITEM_PRICE,
                    Category = Category.Laptops.ToString(),
                    QuantityForSale = ITEM_QUANTITY_FOR_SALE
                }
            };
            this.mapper.Setup(m => m.Map<Item[], MarketplaceItemDto[]>(It.IsAny<Item[]>())).Returns(this.marketplace);

            this.inventory = new InventoryItemDto[] 
            {
                new InventoryItemDto()
                {
                    Code = ITEM_CODE,
                    Name = ITEM_NAME,
                    Category = Category.Laptops.ToString(),
                    QuantityForSale = ITEM_QUANTITY_FOR_SALE,
                    AvailableQuantity = ITEM_AVAILABLE_QUANTITY,
                    QuantityCombined = ITEM_QUANTITY_COMBINED
                }
            };
            this.mapper.Setup(m => m.Map<Item[], InventoryItemDto[]>(It.IsAny<Item[]>())).Returns(this.inventory);

            this.itemDetailsDto = new ItemDetailsDto()
            {
                ImageUrl = ITEM_IMAGE_URL,
                Name = ITEM_NAME,
                Price = ITEM_PRICE,
                Category = Category.Laptops.ToString(),
                QuantityForSale = ITEM_QUANTITY_FOR_SALE,
                Description = ITEM_DESCRIPTION
            };
            this.mapper.Setup(m => m.Map<Item, ItemDetailsDto>(this.item)).Returns(this.itemDetailsDto);

            this.createItemDto = new CreateItemDto()
            {
                Code = ITEM_CODE,
                Name = ITEM_NAME,
                Price = ITEM_PRICE,
                Category = Category.Laptops.ToString(),
                Quantity = ITEM_QUANTITY_COMBINED,
                QuantityForSale = ITEM_QUANTITY_FOR_SALE,
                AvailableQuantity = ITEM_AVAILABLE_QUANTITY,
                Description = ITEM_DESCRIPTION
            };
            this.mapper.Setup(m => m.Map<CreateItemDto, Item>(this.createItemDto)).Returns(this.item);
            this.mapper.Setup(m => m.Map<Item, CreateItemDto>(this.item)).Returns(this.createItemDto);

            this.updateItemDto = new UpdateItemDto()
            {
                Code = ITEM_CODE,
                Name = ITEM_NAME,
                Price = ITEM_PRICE,
                Category = Category.Laptops.ToString(),
                Quantity = ITEM_QUANTITY_COMBINED,
                QuantityForSale = ITEM_QUANTITY_FOR_SALE,
                AvailableQuantity = ITEM_AVAILABLE_QUANTITY,
                Description = ITEM_DESCRIPTION
            };
            this.mapper.Setup(m => m.Map<UpdateItemDto, Item>(this.updateItemDto)).Returns(this.item);
            this.mapper.Setup(m => m.Map<Item, UpdateItemDto>(this.item)).Returns(this.updateItemDto);

            Item[] items = new Item[] { this.item };

            this.itemRepository.Setup(ir => ir.GetMarketplace()).Returns(items);
            this.itemRepository.Setup(ir => ir.GetInventory()).Returns(items);

            this.orderRepository.Setup(or => or.DeclineAllPendingOrdersWithDeletedItemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));

            this.imageService.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            this.imageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>())).ReturnsAsync(ITEM_IMAGE_PUBLIC_ID);
        }

        [Test]
        public void GetMarketplace_Cached_ReturnsMarkeplaceItemDtoArrayMappedFromRepository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<MarketplaceItemDto[]>(It.IsAny<string>())).Returns((MarketplaceItemDto[])null);

            // Act
            MarketplaceItemDto[] marketplace = this.itemService.GetMarketplace();

            // Assert
            marketplace.Should().BeEquivalentTo(this.marketplace);
        }

        [Test]
        public void GetMarketplace_NotCached_ReturnsMarkeplaceItemDtoArrayFromCache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<MarketplaceItemDto[]>(It.IsAny<string>())).Returns(this.marketplace);

            // Act
            MarketplaceItemDto[] marketplace = this.itemService.GetMarketplace();

            // Assert
            marketplace.Should().BeEquivalentTo(this.marketplace);
        }

        [Test]
        public void GetInventory_NotCached_ReturnsInventoryItemDtoArrayMappedFromRepository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<InventoryItemDto[]>(It.IsAny<string>())).Returns((InventoryItemDto[])null);

            // Act
            InventoryItemDto[] inventory = this.itemService.GetInventory();

            // Assert
            inventory.Should().BeEquivalentTo(this.inventory);
        }

        [Test]
        public void GetInventory_Cached_ReturnsInventoryItemDtoArrayFromCache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<InventoryItemDto[]>(It.IsAny<string>())).Returns(this.inventory);

            // Act
            InventoryItemDto[] inventory = this.itemService.GetInventory();

            // Assert
            inventory.Should().BeEquivalentTo(this.inventory);
        }

        [Test]
        public void GetItemByCode_NotCached_ReturnsItemDetailsDtoMappedFromRepository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns((ItemDetailsDto)null);
            this.itemRepository.Setup(ir => ir.GetById(ITEM_ID)).Returns(this.item);

            // Act
            ItemDetailsDto itemDetailsDto = this.itemService.GetById(ITEM_ID);

            // Assert
            itemDetailsDto.Should().Be(this.itemDetailsDto);
        }

        [Test]
        public void GetItemByCode_Cached_ReturnsItemDetailsDtoFromCache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns(this.itemDetailsDto);
            this.itemRepository.Setup(ir => ir.GetById(ITEM_ID)).Returns(this.item);

            // Act
            ItemDetailsDto itemDetailsDto = this.itemService.GetById(ITEM_ID);

            // Assert
            itemDetailsDto.Should().BeEquivalentTo(this.itemDetailsDto);
        }

        [Test]
        public void GetItemByCode_NonExistingItem_ThrowsNotFoundException()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns((ItemDetailsDto) null);
            this.itemRepository.Setup(ir => ir.GetById(ITEM_ID)).Returns((Item) null);

            // Act
            Action action = () => this.itemService.GetById(ITEM_ID);

            // Assert
            action.Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task CreateAsync_UploadImageAsyncWhenOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            this.createItemDto.Image = new Mock<IFormFile>().Object;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.imageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.CreateAsync(this.createItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task CreateAsync_CreateAsyncInItemRepositoryWhenOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemRepository.Setup(s => s.CreateAsync(It.IsAny<Item>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.CreateAsync(this.createItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task CreateAsync_ValidItem_DoesNotThrowException()
        {
            // Arrange
            this.createItemDto.Quantity = ITEM_QUANTITY_COMBINED;
            this.item.QuantityCombined = ITEM_QUANTITY_COMBINED;

            // Act
            Func<Task> task = async () => await itemService.CreateAsync(this.createItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().NotThrowAsync();
        }

        [Test]
        public async Task UpdateAsync_GetPendingOrdersTotalItemQuantityByItemIdAsyncWhenOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ITEM_ID, this.updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_GetItemLoansTotalQuantityForItemAsyncWhenOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItemAsync(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ITEM_ID, this.updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_GetItemImagePublicIdAsyncWhenOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemRepository.Setup(ir => ir.GetItemImagePublicIdAsync(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ITEM_ID, this.updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_UploadAsyncWhenOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            this.updateItemDto.Image = new Mock<IFormFile>().Object;
            this.updateItemDto.QuantityForSale = ITEM_QUANTITY_FOR_SALE;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            this.imageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ITEM_ID, this.updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_UpdateAsyncInImageServiceWhenOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            this.updateItemDto.Image = new Mock<IFormFile>().Object;
            this.updateItemDto.ImageChanges = true;
            this.updateItemDto.QuantityForSale = 1;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            this.itemRepository.Setup(ir => ir.GetItemImagePublicIdAsync(It.IsAny<string>(), cancellationToken)).ReturnsAsync(ITEM_IMAGE_PUBLIC_ID);
            this.imageService.Setup(s => s.UpdateAsync(ITEM_IMAGE_PUBLIC_ID, It.IsAny<IFormFile>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ITEM_ID, this.updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_PendingOrdersTotalItemQuantityGreaterThanOrEqualToNewQuantityCombined_ThrowsArgumentException()
        {
            // Arrange
            this.updateItemDto.Quantity = ITEM_QUANTITY_COMBINED;
            this.item.QuantityCombined = ITEM_QUANTITY_COMBINED;
            this.orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 2);
            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 0);

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ITEM_ID, this.updateItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task UpdateAsync_ItemLoansTotalQuantityForItemGreaterThanOrEqualToNewQuantityCombined_ThrowsArgumentException()
        {
            // Arrange
            this.updateItemDto.Quantity = ITEM_QUANTITY_COMBINED;
            this.item.QuantityCombined = ITEM_QUANTITY_COMBINED;
            this.orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 2);
            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 0);

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ITEM_ID, this.updateItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task UpdateAsync_PendingOrdersTotalItemQuantityAndItemLoansTotalQuantityAreZeros_DoesNotThrowException()
        {
            // Arrange
            this.updateItemDto.Quantity = ITEM_QUANTITY_COMBINED;
            this.item.QuantityCombined = ITEM_QUANTITY_COMBINED;
            this.orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 0);
            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 0);

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ITEM_ID, this.updateItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().NotThrowAsync();
        }

        [Test]
        public async Task DeleteAsync_IsLoanWithItemAsync_ThrowsInvalidOperationException()
        {
            // Arrange
            this.itemLoanRepository.Setup(ilr => ilr.IsLoanWithItemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            Func<Task> task = async () => await this.itemService.DeleteAsync(ITEM_ID, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test]
        public async Task DeleteAsync_IsLoanWithItemAsyncOperationCanceled_hrowsOperationCanceledException()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemLoanRepository.Setup(ilr => ilr.IsLoanWithItemAsync(It.IsAny<string>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.DeleteAsync(ITEM_ID, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task DeleteAsync_GetItemImagePubicIdAsyncOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemLoanRepository.Setup(ilr => ilr.IsLoanWithItemAsync(It.IsAny<string>(), cancellationToken)).ReturnsAsync(false);
            this.itemRepository.Setup(ir => ir.GetItemImagePublicIdAsync(It.IsAny<string>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.DeleteAsync(ITEM_ID, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task DeleteAsync_DeclineAllPendingOrdersWithDeletedItemAsyncOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemLoanRepository.Setup(ilr => ilr.IsLoanWithItemAsync(It.IsAny<string>(), cancellationToken)).ReturnsAsync(false);
            this.itemRepository.Setup(ir => ir.GetItemImagePublicIdAsync(It.IsAny<string>(), cancellationToken)).ReturnsAsync(ITEM_IMAGE_PUBLIC_ID);
            this.orderRepository.Setup(ir => ir.DeclineAllPendingOrdersWithDeletedItemAsync(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.DeleteAsync(ITEM_ID, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public void DeleteAsync_ExistingAndValidItem_DoesNotThrowException()
        {
            // Arrange

            // Act
            Func<Task> task = async () => await this.itemService.DeleteAsync(ITEM_ID, It.IsAny<CancellationToken>());

            // Assert
            task.Should().NotThrowAsync();
        }
    }
}