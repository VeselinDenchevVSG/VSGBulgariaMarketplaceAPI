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
    using Test.Constants;

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
            this.itemService = new ItemService(this.itemRepository.Object, this.orderRepository.Object, this.itemLoanRepository.Object,
                                    this.imageService.Object, this.memoryCache.Object, this.mapper.Object);
            this.item = new Item()
            {
                Id = ItemConstant.ITEM_ID,
                Code = ItemConstant.ITEM_CODE,
                Name = ItemConstant.ITEM_NAME,
                ImagePublicId = ItemConstant.ITEM_IMAGE_PUBLIC_ID,
                Price = ItemConstant.ITEM_PRICE,
                Category = Category.Laptops,
                QuantityCombined = ItemConstant.ITEM_QUANTITY_COMBINED,
                AvailableQuantity = ItemConstant.ITEM_AVAILABLE_QUANTITY
            };

            this.marketplace = new MarketplaceItemDto[] 
            {
                new MarketplaceItemDto()
                {
                    Code = ItemConstant.ITEM_CODE,
                    ImageUrl = ItemConstant.ITEM_IMAGE_URL,
                    Price = ItemConstant.ITEM_PRICE,
                    Category = Category.Laptops.ToString(),
                    QuantityForSale = ItemConstant.ITEM_QUANTITY_FOR_SALE
                }
            };
            this.mapper.Setup(m => m.Map<Item[], MarketplaceItemDto[]>(It.IsAny<Item[]>())).Returns(this.marketplace);

            this.inventory = new InventoryItemDto[] 
            {
                new InventoryItemDto()
                {
                    Code = ItemConstant.ITEM_CODE,
                    Name = ItemConstant.ITEM_NAME,
                    Category = Category.Laptops.ToString(),
                    QuantityForSale = ItemConstant.ITEM_QUANTITY_FOR_SALE,
                    AvailableQuantity = ItemConstant.ITEM_AVAILABLE_QUANTITY,
                    QuantityCombined = ItemConstant.ITEM_QUANTITY_COMBINED
                }
            };
            this.mapper.Setup(m => m.Map<Item[], InventoryItemDto[]>(It.IsAny<Item[]>())).Returns(this.inventory);

            this.itemDetailsDto = new ItemDetailsDto()
            {
                ImageUrl = ItemConstant.ITEM_IMAGE_URL,
                Name = ItemConstant.ITEM_NAME,
                Price = ItemConstant.ITEM_PRICE,
                Category = Category.Laptops.ToString(),
                QuantityForSale = ItemConstant.ITEM_QUANTITY_FOR_SALE,
                Description = ItemConstant.ITEM_DESCRIPTION
            };
            this.mapper.Setup(m => m.Map<Item, ItemDetailsDto>(this.item)).Returns(this.itemDetailsDto);

            this.createItemDto = new CreateItemDto()
            {
                Code = ItemConstant.ITEM_CODE,
                Name = ItemConstant.ITEM_NAME,
                Price = ItemConstant.ITEM_PRICE,
                Category = Category.Laptops.ToString(),
                Quantity = ItemConstant.ITEM_QUANTITY_COMBINED,
                QuantityForSale = ItemConstant.ITEM_QUANTITY_FOR_SALE,
                AvailableQuantity = ItemConstant.ITEM_AVAILABLE_QUANTITY,
                Description = ItemConstant.ITEM_DESCRIPTION
            };
            this.mapper.Setup(m => m.Map<CreateItemDto, Item>(this.createItemDto)).Returns(this.item);
            this.mapper.Setup(m => m.Map<Item, CreateItemDto>(this.item)).Returns(this.createItemDto);

            this.updateItemDto = new UpdateItemDto()
            {
                Code = ItemConstant.ITEM_CODE,
                Name = ItemConstant.ITEM_NAME,
                Price = ItemConstant.ITEM_PRICE,
                Category = Category.Laptops.ToString(),
                Quantity = ItemConstant.ITEM_QUANTITY_COMBINED,
                QuantityForSale = ItemConstant.ITEM_QUANTITY_FOR_SALE,
                AvailableQuantity = ItemConstant.ITEM_AVAILABLE_QUANTITY,
                Description = ItemConstant.ITEM_DESCRIPTION
            };
            this.mapper.Setup(m => m.Map<UpdateItemDto, Item>(this.updateItemDto)).Returns(this.item);
            this.mapper.Setup(m => m.Map<Item, UpdateItemDto>(this.item)).Returns(this.updateItemDto);

            Item[] items = new Item[] { this.item };

            this.itemRepository.Setup(ir => ir.GetMarketplace()).Returns(items);
            this.itemRepository.Setup(ir => ir.GetInventory()).Returns(items);

            this.orderRepository.Setup(or => or.DeclineAllPendingOrdersWithDeletedItemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));

            this.imageService.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            this.imageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>())).ReturnsAsync(ItemConstant.ITEM_IMAGE_PUBLIC_ID);
        }

        [Test]
        public void GetMarketplace_Should_Return_MarkeplaceItemDtoArray_Mapped_From_Repository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<MarketplaceItemDto[]>(It.IsAny<string>())).Returns((MarketplaceItemDto[])null);

            // Act
            MarketplaceItemDto[] marketplace = this.itemService.GetMarketplace();

            // Assert
            marketplace.Should().BeEquivalentTo(this.marketplace);
        }

        [Test]
        public void GetMarketplace_Should_Return_MarkeplaceItemDtoArray_From_Cache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<MarketplaceItemDto[]>(It.IsAny<string>())).Returns(this.marketplace);

            // Act
            MarketplaceItemDto[] marketplace = this.itemService.GetMarketplace();

            // Assert
            marketplace.Should().BeEquivalentTo(this.marketplace);
        }

        [Test]
        public void GetInventory_Should_Return_InventoryItemDtoArray_Mapped_From_Repository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<InventoryItemDto[]>(It.IsAny<string>())).Returns((InventoryItemDto[])null);

            // Act
            InventoryItemDto[] inventory = this.itemService.GetInventory();

            // Assert
            inventory.Should().BeEquivalentTo(this.inventory);
        }

        [Test]
        public void GetInventory_Should_Return_InventoryItemDtoArray_From_Cache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<InventoryItemDto[]>(It.IsAny<string>())).Returns(this.inventory);

            // Act
            InventoryItemDto[] inventory = this.itemService.GetInventory();

            // Assert
            inventory.Should().BeEquivalentTo(this.inventory);
        }

        [Test]
        public void GetItemByCode_Should_Return_ItemDetailsDto_Mapped_From_Repository()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns((ItemDetailsDto)null);
            this.itemRepository.Setup(ir => ir.GetById(ItemConstant.ITEM_ID)).Returns(this.item);

            // Act
            ItemDetailsDto itemDetailsDto = this.itemService.GetById(ItemConstant.ITEM_ID);

            // Assert
            itemDetailsDto.Should().Be(this.itemDetailsDto);
        }

        [Test]
        public void GetItemByCode_Should_Return_ItemDetailsDto_From_Cache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns(this.itemDetailsDto);
            this.itemRepository.Setup(ir => ir.GetById(ItemConstant.ITEM_ID)).Returns(this.item);

            // Act
            ItemDetailsDto itemDetailsDto = this.itemService.GetById(ItemConstant.ITEM_ID);

            // Assert
            itemDetailsDto.Should().BeEquivalentTo(this.itemDetailsDto);
        }

        [Test]
        public void GetItemByCode_Should_Throw_NotFoundException()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns((ItemDetailsDto) null);
            this.itemRepository.Setup(ir => ir.GetById(ItemConstant.ITEM_ID)).Returns((Item) null);

            // Act
            Action action = () => this.itemService.GetById(ItemConstant.ITEM_ID);

            // Assert
            action.Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task CreateAsync_Uploading_Image_Async_Should_Throw_OperationCanceledException_When_Operation_Is_Canceled()
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
        public async Task CreateAsync_Inserting_Item_Into_Database_Should_Throw_OperationCanceledException_When_Operation_Is_Canceled()
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
        public async Task CreateAsync_Should_Not_Throw_Exception()
        {
            // Arrange
            this.createItemDto.Quantity = ItemConstant.ITEM_QUANTITY_COMBINED;
            this.item.QuantityCombined = ItemConstant.ITEM_QUANTITY_COMBINED;

            // Act
            Func<Task> task = async () => await itemService.CreateAsync(this.createItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().NotThrowAsync();
        }

        [Test]
        public async Task UpdateAsync_Getting_Pending_Orders_Total_Item_Quantity_By_Item_Id_Should_Throw_OperationCanceledException_When_Operation_Is_Canceled()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemId(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ItemConstant.ITEM_ID, this.updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_Getting_Item_Loans_Total_Item_Quantity_For_Item_Should_Throw_OperationCanceledException_When_Operation_Is_Canceled()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItem(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ItemConstant.ITEM_ID, this.updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_Getting_Item_Image_Public_Id_Should_Throw_OperationCanceledException_When_Operation_Is_Canceled()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemRepository.Setup(ir => ir.GetItemImagePublicId(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ItemConstant.ITEM_ID, this.updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_Uploading_Image_Async_Should_Throw_OperationCanceledException_When_Before_Was_No_Image_Operation_Is_Canceled()
        {
            // Arrange
            this.updateItemDto.Image = new Mock<IFormFile>().Object;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.imageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ItemConstant.ITEM_ID, this.updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_Uploading_Image_Async_Should_Throw_OperationCanceledException_When_Image_Changes_Operation_Is_Canceled()
        {
            // Arrange
            this.updateItemDto.Image = new Mock<IFormFile>().Object;
            this.updateItemDto.ImageChanges = true;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemRepository.Setup(ir => ir.GetItemImagePublicId(It.IsAny<string>(), cancellationToken)).ReturnsAsync(ItemConstant.ITEM_IMAGE_PUBLIC_ID);
            this.imageService.Setup(s => s.UpdateAsync(ItemConstant.ITEM_IMAGE_PUBLIC_ID, It.IsAny<IFormFile>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ItemConstant.ITEM_ID, this.updateItemDto, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task UpdateAsync_Pending_Orders_Total_Item_Quantity_Greater_Or_Equal_To_New_Quantity_Combined_Should_Throw_ArgumentException()
        {
            // Arrange
            this.updateItemDto.Quantity = ItemConstant.ITEM_QUANTITY_COMBINED;
            this.item.QuantityCombined = ItemConstant.ITEM_QUANTITY_COMBINED;
            this.orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemId(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 2);
            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItem(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 0);

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ItemConstant.ITEM_ID, this.updateItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task UpdateAsync_Item_Loans_Total_Quantity_For_Item_Greater_Or_Equal_To_New_Quantity_Combined_Should_Throw_ArgumentException()
        {
            // Arrange
            this.updateItemDto.Quantity = ItemConstant.ITEM_QUANTITY_COMBINED;
            this.item.QuantityCombined = ItemConstant.ITEM_QUANTITY_COMBINED;
            this.orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemId(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 2);
            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItem(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 0);

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ItemConstant.ITEM_ID, this.updateItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task UpdateAsync_Should_Not_Throw_Exception()
        {
            // Arrange
            this.updateItemDto.Quantity = ItemConstant.ITEM_QUANTITY_COMBINED;
            this.item.QuantityCombined = ItemConstant.ITEM_QUANTITY_COMBINED;
            this.orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemId(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 0);
            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItem(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 0);

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ItemConstant.ITEM_ID, this.updateItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().NotThrowAsync();
        }

        [Test]
        public async Task UpdateAsyncItem_Should_Not_Throw_Exception()
        {
            // Arrange
            this.updateItemDto.Quantity = ItemConstant.ITEM_QUANTITY_COMBINED;
            this.item.QuantityCombined = ItemConstant.ITEM_QUANTITY_COMBINED;
            this.orderRepository.Setup(or => or.GetPendingOrdersTotalItemQuantityByItemId(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 0);
            this.itemLoanRepository.Setup(ilr => ilr.GetItemLoansTotalQuantityForItem(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((short) 0);

            // Act
            Func<Task> task = async () => await itemService.UpdateAsync(ItemConstant.ITEM_ID, this.updateItemDto, It.IsAny<CancellationToken>());

            // Assert
            await task.Should().NotThrowAsync();
        }

        [Test]
        public async Task DeleteAsync_IsLoanWithItem_Should_Throw_OperationCanceledException_When_Image_Changes_Operation_Is_Canceled()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemLoanRepository.Setup(ilr => ilr.IsLoanWithItem(It.IsAny<string>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.DeleteAsync(ItemConstant.ITEM_ID, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task DeleteAsync_Getting_Item_Image_Public_Id_Should_Throw_OperationCanceledException_When_Image_Changes_Operation_Is_Canceled()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemLoanRepository.Setup(ilr => ilr.IsLoanWithItem(It.IsAny<string>(), cancellationToken)).ReturnsAsync(false);
            this.itemRepository.Setup(ir => ir.GetItemImagePublicId(It.IsAny<string>(), cancellationToken)).ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.DeleteAsync(ItemConstant.ITEM_ID, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public async Task DeleteAsync_Declining_All_Pending_Orders_Should_Throw_OperationCanceledException_When_Image_Changes_Operation_Is_Canceled()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.itemLoanRepository.Setup(ilr => ilr.IsLoanWithItem(It.IsAny<string>(), cancellationToken)).ReturnsAsync(false);
            this.itemRepository.Setup(ir => ir.GetItemImagePublicId(It.IsAny<string>(), cancellationToken)).ReturnsAsync(ItemConstant.ITEM_IMAGE_PUBLIC_ID);
            this.orderRepository.Setup(ir => ir.DeclineAllPendingOrdersWithDeletedItemAsync(It.IsAny<string>(), cancellationToken))
                                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> task = async () => await this.itemService.DeleteAsync(ItemConstant.ITEM_ID, cancellationToken);

            // Assert
            await task.Should().ThrowAsync<OperationCanceledException>();
        }

        [Test]
        public void DeleteItem_Should_Not_Throw_Exception()
        {
            // Arrange

            // Act
            Func<Task> task = async () => await this.itemService.DeleteAsync(ItemConstant.ITEM_ID, It.IsAny<CancellationToken>());

            // Assert
            task.Should().NotThrowAsync();
        }
    }
}