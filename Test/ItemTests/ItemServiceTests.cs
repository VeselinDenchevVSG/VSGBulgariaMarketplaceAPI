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

    public class ItemServiceTests
    {
        private const string ITEM_ID = "Test";
        private const string ITEM_CODE = "Test";
        private const string ITEM_NAME = "Test";
        private const string ITEM_IMAGE_PUBLIC_ID = "Test";
        private const string ITEM_IMAGE_URL = "https://shorturl.at/fgwFK";
        private const decimal ITEM_PRICE = 1.11m;
        private const short ITEM_QUANTITY_COMBINED = 1;
        private const short ITEM_QUANTITY_FOR_SALE = 1;
        private const string ITEM_DESCRIPTION = "Test";

        private readonly Mock<IItemRepository> itemRepository;
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
            this.imageService = new Mock<ICloudImageService>();
            this.memoryCache = new Mock<IMemoryCacheAdapter>();
            this.mapper = new Mock<IMapper>();
            this.itemService = new ItemService(this.itemRepository.Object, this.orderRepository.Object, this.imageService.Object, this.memoryCache.Object, this.mapper.Object);
            this.item = new Item()
            {
                Id = ITEM_ID,
                Code = ITEM_CODE,
                Name = ITEM_NAME,
                ImagePublicId = ITEM_IMAGE_PUBLIC_ID,
                Price = ITEM_PRICE,
                Category = Category.Laptops,
                QuantityCombined = ITEM_QUANTITY_COMBINED
            };

            MarketplaceItemDto marketplaceItemDto = new MarketplaceItemDto()
            {
                Code = ITEM_CODE,
                ImageUrl = ITEM_IMAGE_URL,
                Price = ITEM_PRICE,
                Category = Category.Laptops.ToString(),
                QuantityForSale = ITEM_QUANTITY_FOR_SALE
            };
            this.marketplace = new MarketplaceItemDto[] { marketplaceItemDto };
            this.mapper.Setup(m => m.Map<Item[], MarketplaceItemDto[]>(It.IsAny<Item[]>())).Returns(this.marketplace);

            InventoryItemDto inventoryItemDto = new InventoryItemDto()
            {
                Code = ITEM_CODE,
                Name = ITEM_NAME,
                Category = Category.Laptops.ToString(),
                QuantityForSale = ITEM_QUANTITY_FOR_SALE,
                QuantityCombined = ITEM_QUANTITY_COMBINED
            };
            this.inventory = new InventoryItemDto[] { inventoryItemDto };
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
            this.mapper.Setup(m => m.Map<Item, ItemDetailsDto>(It.IsAny<Item>())).Returns(this.itemDetailsDto);

            this.createItemDto = new CreateItemDto()
            {
                Code = ITEM_CODE,
                Name = ITEM_NAME,
                Price = ITEM_PRICE,
                Category = Category.Laptops.ToString(),
                Quantity = ITEM_QUANTITY_COMBINED,
                QuantityForSale = ITEM_QUANTITY_FOR_SALE,
                Description = ITEM_DESCRIPTION
            };
            this.mapper.Setup(m => m.Map<CreateItemDto, Item>(It.IsAny<CreateItemDto>())).Returns(this.item);
            this.mapper.Setup(m => m.Map<Item, CreateItemDto>(It.IsAny<Item>())).Returns(this.createItemDto);

            this.updateItemDto = new UpdateItemDto()
            {
                Code = ITEM_CODE,
                Name = ITEM_NAME,
                Price = ITEM_PRICE,
                Category = Category.Laptops.ToString(),
                Quantity = ITEM_QUANTITY_COMBINED,
                QuantityForSale = ITEM_QUANTITY_FOR_SALE,
                Description = ITEM_DESCRIPTION
            };
            this.mapper.Setup(m => m.Map<CreateItemDto, Item>(It.IsAny<CreateItemDto>())).Returns(this.item);
            this.mapper.Setup(m => m.Map<Item, UpdateItemDto>(It.IsAny<Item>())).Returns(this.updateItemDto);

            Item[] items = new Item[] { this.item };

            this.itemRepository.Setup(ir => ir.GetMarketplace()).Returns(items);
            this.itemRepository.Setup(ir => ir.GetInventory()).Returns(items);
            this.itemRepository.Setup(ir => ir.Create(It.IsAny<Item>()));
            this.itemRepository.Setup(ir => ir.Update(It.IsAny<string>(), It.IsAny<Item>()));
            this.itemRepository.Setup(ir => ir.DeleteById(It.IsAny<string>()));

            this.orderRepository.Setup(or => or.DeclineAllPendingOrdersWithDeletedItem(It.IsAny<string>()));

            this.imageService.Setup(s => s.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            this.imageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync("https://shorturl.at/fgwFK");
            this.imageService.Setup(s => s.UpdateAsync(It.IsAny<string>(), It.IsAny<IFormFile>()));
            this.imageService.Setup(s => s.DeleteAsync(It.IsAny<string>()));

            this.memoryCache.Setup(mc => mc.Set(It.IsAny<string>(), It.IsAny<object>()));
            this.memoryCache.Setup(mc => mc.Remove(It.IsAny<string>()));
            this.memoryCache.Setup(mc => mc.Clear());
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
        public void GetMarketplace_Should_Return_MarkeplaceItemDtoArray_Mapped_From_Cache()
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
        public void GetInventory_Should_Return_InventoryItemDtoArray_Mapped_From_Cache()
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
            this.itemRepository.Setup(ir => ir.GetById(It.IsAny<string>())).Returns(this.item);

            // Act
            ItemDetailsDto itemDetailsDto = this.itemService.GetById(ITEM_ID);

            // Assert
            itemDetailsDto.Should().Be(this.itemDetailsDto);
        }

        [Test]
        public void GetItemByCode_Should_Return_ItemDetailsDto_Mapped_From_Cache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns(this.itemDetailsDto);
            this.itemRepository.Setup(ir => ir.GetById(It.IsAny<string>())).Returns(this.item);

            // Act
            ItemDetailsDto itemDetailsDto = this.itemService.GetById(ITEM_ID);

            // Assert
            itemDetailsDto.Should().BeEquivalentTo(this.itemDetailsDto);
        }

        [Test]
        public void GetItemByCode_Should_Throw_NotFoundException()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns((ItemDetailsDto) null);
            this.itemRepository.Setup(ir => ir.GetById(It.IsAny<string>())).Returns((Item) null);

            // Act
            Action action = () => this.itemService.GetById(ITEM_ID);

            // Assert
            action.Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task CreateAsync_Should_Not_Throw_Exception()
        {
            // Arrange
            this.createItemDto.Quantity = ITEM_QUANTITY_COMBINED;

            // Act
            Func<Task> task = async () => await itemService.CreateAsync(this.createItemDto);

            // Assert
            await task.Should().NotThrowAsync();
        }

        [Test]
        public async Task CreateAsync_Should_Throw_ArgumentOutOfRangeException()
        {
            // Arrange
            this.createItemDto.Quantity = 0;

            // Act
            Func<Task> task = async () => await this.itemService.CreateAsync(this.createItemDto);

            // Assert
            await task.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Test]
        public async Task UpdateAsync_Should_Not_Throw_Exception()
        {
            // Arrange
            this.updateItemDto.Quantity = ITEM_QUANTITY_COMBINED;

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ITEM_ID, this.updateItemDto);

            // Assert
            await task.Should().NotThrowAsync();
        }

        [Test]
        public async Task UpdateAsync_Should_Throw_ArgumentOutOfRangeException()
        {
            // Arrange
            this.updateItemDto.Quantity = 0;

            // Act
            Func<Task> task = async () => await itemService.UpdateAsync(ITEM_ID, this.updateItemDto);

            // Assert
            await task.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Test]
        public async Task UpdateAsyncItem_Should_Not_Throw_Exception()
        {
            // Arrange
            this.updateItemDto.Quantity = ITEM_QUANTITY_COMBINED;

            // Act
            Func<Task> task = async () => await itemService.UpdateAsync(ITEM_ID, this.updateItemDto);

            // Assert
            await task.Should().NotThrowAsync();
        }

        [Test]
        public void DeleteItem_Should_Not_Throw_Exception()
        {
            // Arrange

            // Act
            Action action = () => this.itemService.Delete(ITEM_ID);

            // Assert
            action.Should().NotThrow();
        }
    }
}