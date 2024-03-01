namespace Test.UnitTests.Item
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
    using static VSGBulgariaMarketplace.Application.Constants.ServiceConstant;

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
            itemRepository = new Mock<IItemRepository>();
            orderRepository = new Mock<IOrderRepository>();
            itemLoanRepository = new Mock<IItemLoanRepository>();
            imageService = new Mock<ICloudImageService>();
            memoryCache = new Mock<IMemoryCacheAdapter>();
            mapper = new Mock<IMapper>();
            itemService = new ItemService(itemRepository.Object, orderRepository.Object, itemLoanRepository.Object, imageService.Object,
                                                memoryCache.Object, mapper.Object);
            item = new Item()
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

            Item[] items = new Item[] { item };

            marketplace = new MarketplaceItemDto[]
            {
                new MarketplaceItemDto()
                {
                    Code = ITEM_CODE,
                    ImageUrl = ITEM_IMAGE_URL,
                    Price = ITEM_PRICE,
                    Category = nameof(Category.Laptops),
                    QuantityForSale = ITEM_QUANTITY_FOR_SALE
                }
            };
            mapper.Setup(m => m.Map<Item[], MarketplaceItemDto[]>(items)).Returns(marketplace);

            inventory = new InventoryItemDto[]
            {
                new InventoryItemDto()
                {
                    Code = ITEM_CODE,
                    Name = ITEM_NAME,
                    Category = nameof(Category.Laptops),
                    QuantityForSale = ITEM_QUANTITY_FOR_SALE,
                    AvailableQuantity = ITEM_AVAILABLE_QUANTITY,
                    QuantityCombined = ITEM_QUANTITY_COMBINED
                }
            };
            mapper.Setup(m => m.Map<Item[], InventoryItemDto[]>(items)).Returns(inventory);

            itemDetailsDto = new ItemDetailsDto()
            {
                ImageUrl = ITEM_IMAGE_URL,
                Name = ITEM_NAME,
                Price = ITEM_PRICE,
                Category = nameof(Category.Laptops),
                QuantityForSale = ITEM_QUANTITY_FOR_SALE,
                Description = ITEM_DESCRIPTION
            };
            mapper.Setup(m => m.Map<Item, ItemDetailsDto>(item)).Returns(itemDetailsDto);

            createItemDto = new CreateItemDto()
            {
                Code = ITEM_CODE,
                Name = ITEM_NAME,
                Price = ITEM_PRICE,
                Category = nameof(Category.Laptops),
                Quantity = ITEM_QUANTITY_COMBINED,
                QuantityForSale = ITEM_QUANTITY_FOR_SALE,
                AvailableQuantity = ITEM_AVAILABLE_QUANTITY,
                Description = ITEM_DESCRIPTION
            };
            mapper.Setup(m => m.Map<CreateItemDto, Item>(createItemDto)).Returns(item);
            mapper.Setup(m => m.Map<Item, CreateItemDto>(item)).Returns(createItemDto);

            updateItemDto = new UpdateItemDto()
            {
                Code = ITEM_CODE,
                Name = ITEM_NAME,
                Price = ITEM_PRICE,
                Category = nameof(Category.Laptops),
                Quantity = ITEM_QUANTITY_COMBINED,
                QuantityForSale = ITEM_QUANTITY_FOR_SALE,
                AvailableQuantity = ITEM_AVAILABLE_QUANTITY,
                Description = ITEM_DESCRIPTION
            };
            mapper.Setup(m => m.Map<UpdateItemDto, Item>(updateItemDto)).Returns(item);
            mapper.Setup(m => m.Map<Item, UpdateItemDto>(item)).Returns(updateItemDto);


            itemRepository.Setup(ir => ir.GetMarketplace()).Returns(items);
            itemRepository.Setup(ir => ir.GetInventory()).Returns(items);

            orderRepository.Setup(or => or.DeclineAllPendingOrdersWithDeletedItemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));

            imageService.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            imageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>())).ReturnsAsync(ITEM_IMAGE_PUBLIC_ID);
        }

        [Test]
        public void GetMarketplace_Cached_ReturnsMarkeplaceItemDtoArrayMappedFromRepository()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<MarketplaceItemDto[]>(It.IsAny<string>())).Returns((MarketplaceItemDto[])null);

            // Act
            MarketplaceItemDto[] marketplace = itemService.GetMarketplace();

            // Assert
            marketplace.Should().BeEquivalentTo(this.marketplace);
        }

        [Test]
        public void GetMarketplace_NotCached_ReturnsMarkeplaceItemDtoArrayFromCache()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<MarketplaceItemDto[]>(It.IsAny<string>())).Returns(this.marketplace);

            // Act
            MarketplaceItemDto[] marketplace = itemService.GetMarketplace();

            // Assert
            marketplace.Should().BeEquivalentTo(this.marketplace);
        }

        [Test]
        public void GetInventory_NotCached_ReturnsInventoryItemDtoArrayMappedFromRepository()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<InventoryItemDto[]>(It.IsAny<string>())).Returns((InventoryItemDto[])null);

            // Act
            InventoryItemDto[] inventory = itemService.GetInventory();

            // Assert
            inventory.Should().BeEquivalentTo(this.inventory);
        }

        [Test]
        public void GetInventory_Cached_ReturnsInventoryItemDtoArrayFromCache()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<InventoryItemDto[]>(It.IsAny<string>())).Returns(this.inventory);

            // Act
            InventoryItemDto[] inventory = itemService.GetInventory();

            // Assert
            inventory.Should().BeEquivalentTo(this.inventory);
        }

        [Test]
        public void GetItemByCode_NotCached_ReturnsItemDetailsDtoMappedFromRepository()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns((ItemDetailsDto)null);
            itemRepository.Setup(ir => ir.GetById(ITEM_ID)).Returns(item);

            // Act
            ItemDetailsDto itemDetailsDto = itemService.GetById(ITEM_ID);

            // Assert
            itemDetailsDto.Should().Be(this.itemDetailsDto);
        }

        [Test]
        public void GetItemByCode_Cached_ReturnsItemDetailsDtoFromCache()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns(this.itemDetailsDto);
            itemRepository.Setup(ir => ir.GetById(ITEM_ID)).Returns(item);

            // Act
            ItemDetailsDto itemDetailsDto = itemService.GetById(ITEM_ID);

            // Assert
            itemDetailsDto.Should().BeEquivalentTo(this.itemDetailsDto);
        }

        [Test]
        public void GetItemByCode_NonExistingItem_ThrowsNotFoundException()
        {
            // Arrange
            memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns((ItemDetailsDto)null);
            itemRepository.Setup(ir => ir.GetById(ITEM_ID)).Returns((Item)null);

            // Act
            Action action = () => itemService.GetById(ITEM_ID);

            // Assert
            action.Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task CreateAsync_UploadImageAsyncWhenOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            createItemDto.Image = new Mock<IFormFile>().Object;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            imageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.CreateAsync(createItemDto, cancellationToken);

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

            itemRepository.Setup(s => s.CreateAsync(It.IsAny<Item>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.CreateAsync(createItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task CreateAsync_ValidItem_DoesNotThrowException()
        {
            // Arrange
            createItemDto.Quantity = ITEM_QUANTITY_COMBINED;
            item.QuantityCombined = ITEM_QUANTITY_COMBINED;

            // Act
            Func<Task> task = async () => await itemService.CreateAsync(createItemDto, It.IsAny<CancellationToken>());

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

            orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.UpdateAsync(ITEM_ID, updateItemDto, cancellationToken);

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

            itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItemAsync(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.UpdateAsync(ITEM_ID, updateItemDto, cancellationToken);

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

            itemRepository.Setup(ir => ir.GetItemImagePublicIdAsync(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.UpdateAsync(ITEM_ID, updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_UploadAsyncWhenOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            updateItemDto.Image = new Mock<IFormFile>().Object;
            updateItemDto.QuantityForSale = ITEM_QUANTITY_FOR_SALE;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            imageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.UpdateAsync(ITEM_ID, updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_UpdateAsyncInImageServiceWhenOperationCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            updateItemDto.Image = new Mock<IFormFile>().Object;
            updateItemDto.ImageChanges = true;
            updateItemDto.QuantityForSale = 1;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            itemRepository.Setup(ir => ir.GetItemImagePublicIdAsync(It.IsAny<string>(), cancellationToken)).ReturnsAsync(ITEM_IMAGE_PUBLIC_ID);
            imageService.Setup(s => s.UpdateAsync(CLOUDINARY_IMAGE_DIRECTORY + ITEM_IMAGE_PUBLIC_ID, It.IsAny<IFormFile>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.UpdateAsync(ITEM_ID, updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_PendingOrdersTotalItemQuantityGreaterThanOrEqualToNewQuantityCombined_ThrowsArgumentException()
        {
            // Arrange
            updateItemDto.Quantity = ITEM_QUANTITY_COMBINED;
            item.QuantityCombined = ITEM_QUANTITY_COMBINED;
            orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short)2);
            itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short)0);

            // Act
            Func<Task> task = async () => await itemService.UpdateAsync(ITEM_ID, updateItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task UpdateAsync_ItemLoansTotalQuantityForItemGreaterThanOrEqualToNewQuantityCombined_ThrowsArgumentException()
        {
            // Arrange
            updateItemDto.Quantity = ITEM_QUANTITY_COMBINED;
            item.QuantityCombined = ITEM_QUANTITY_COMBINED;
            orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short)2);
            itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short)0);

            // Act
            Func<Task> task = async () => await itemService.UpdateAsync(ITEM_ID, updateItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task UpdateAsync_PendingOrdersTotalItemQuantityAndItemLoansTotalQuantityAreZeros_DoesNotThrowException()
        {
            // Arrange
            updateItemDto.Quantity = ITEM_QUANTITY_COMBINED;
            item.QuantityCombined = ITEM_QUANTITY_COMBINED;
            orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short)0);
            itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short)0);

            // Act
            Func<Task> task = async () => await itemService.UpdateAsync(ITEM_ID, updateItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().NotThrowAsync();
        }

        [Test]
        public async Task DeleteAsync_IsLoanWithItemAsync_ThrowsInvalidOperationException()
        {
            // Arrange
            itemLoanRepository.Setup(ilr => ilr.IsLoanWithItemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            Func<Task> task = async () => await itemService.DeleteAsync(ITEM_ID, It.IsAny<CancellationToken>());

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

            itemLoanRepository.Setup(ilr => ilr.IsLoanWithItemAsync(It.IsAny<string>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.DeleteAsync(ITEM_ID, cancellationToken);

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

            itemLoanRepository.Setup(ilr => ilr.IsLoanWithItemAsync(It.IsAny<string>(), cancellationToken)).ReturnsAsync(false);
            itemRepository.Setup(ir => ir.GetItemImagePublicIdAsync(It.IsAny<string>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.DeleteAsync(ITEM_ID, cancellationToken);

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

            itemLoanRepository.Setup(ilr => ilr.IsLoanWithItemAsync(It.IsAny<string>(), cancellationToken)).ReturnsAsync(false);
            itemRepository.Setup(ir => ir.GetItemImagePublicIdAsync(It.IsAny<string>(), cancellationToken)).ReturnsAsync(ITEM_IMAGE_PUBLIC_ID);
            orderRepository.Setup(ir => ir.DeclineAllPendingOrdersWithDeletedItemAsync(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await itemService.DeleteAsync(ITEM_ID, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public void DeleteAsync_ExistingAndValidItem_DoesNotThrowException()
        {
            // Arrange

            // Act
            Func<Task> task = async () => await itemService.DeleteAsync(ITEM_ID, It.IsAny<CancellationToken>());

            // Assert
            task.Should().NotThrowAsync();
        }
    }
}