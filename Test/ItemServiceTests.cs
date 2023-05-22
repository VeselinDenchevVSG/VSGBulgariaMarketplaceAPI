namespace Test
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

    public class ItemServiceTests
    {
        private const int ITEM_CODE = 1;

        private readonly Mock<IItemRepository> itemRepository;
        private readonly Mock<ICloudImageService> imageService;
        private readonly Mock<IMemoryCacheAdapter> memoryCache;
        private readonly Mock<IMapper> mapper;

        private readonly ItemService itemService;

        private readonly Item item;
        private readonly MarketplaceItemDto[] marketplace;
        private readonly InventoryItemDto[] inventory;
        private readonly ItemDetailsDto itemDetailsDto;
        private readonly ManageItemDto manageItemDto;

        public ItemServiceTests()
        {
            this.itemRepository = new Mock<IItemRepository>();
            this.imageService = new Mock<ICloudImageService>();
            this.memoryCache = new Mock<IMemoryCacheAdapter>();
            this.mapper = new Mock<IMapper>();
            this.itemService = new ItemService(this.itemRepository.Object, this.imageService.Object, this.memoryCache.Object, this.mapper.Object);
            this.item = new Item()
            {
                Id = ITEM_CODE,
                Name = "Test",
                Image = new CloudinaryImage()
                {
                    Id = "Test",
                    SecureUrl = "https://shorturl.at/fgwFK"
                },
                ImagePublicId = "Test",
                Price = 1.11m,
                Category = Category.Laptops,
                QuantityCombined = 1
            };

            MarketplaceItemDto marketplaceItemDto = new MarketplaceItemDto()
            {
                Code = ITEM_CODE,
                ImageUrl = "https://shorturl.at/fgwFK",
                Price = 1.11m,
                Category = "Laptops",
                QuantityForSale = 1
            };
            this.marketplace = new MarketplaceItemDto[] { marketplaceItemDto };
            this.mapper.Setup(m => m.Map<Item[], MarketplaceItemDto[]>(It.IsAny<Item[]>())).Returns(this.marketplace);

            InventoryItemDto inventoryItemDto = new InventoryItemDto()
            {
                Code = ITEM_CODE,
                Name = "Test",
                Category = "Laptops",
                QuantityForSale = 1,
                QuantityCombined = 1
            };
            this.inventory = new InventoryItemDto[] { inventoryItemDto };
            this.mapper.Setup(m => m.Map<Item[], InventoryItemDto[]>(It.IsAny<Item[]>())).Returns(this.inventory);

            this.itemDetailsDto = new ItemDetailsDto()
            {
                ImageUrl = "https://shorturl.at/fgwFK",
                Name = "Test",
                Price = 1.11m,
                Category = "Laptops",
                QuantityForSale = 1,
                Description = "Test"
            };
            this.mapper.Setup(m => m.Map<Item, ItemDetailsDto>(It.IsAny<Item>())).Returns(this.itemDetailsDto);

            this.manageItemDto = new ManageItemDto()
            {
                Code = ITEM_CODE,
                Name = "Test",
                Price = 1.11m,
                Category = "Laptops",
                QuantityCombined = 1,
                QuantityForSale = 1,
                Description = "Test"
            };
            this.mapper.Setup(m => m.Map<ManageItemDto, Item>(It.IsAny<ManageItemDto>())).Returns(this.item);
            this.mapper.Setup(m => m.Map<Item, ManageItemDto>(It.IsAny<Item>())).Returns(this.manageItemDto);

            Item[] items = new Item[] { this.item };

            this.itemRepository.Setup(ir => ir.GetMarketplace()).Returns(items);
            this.itemRepository.Setup(ir => ir.GetInventory()).Returns(items);
            this.itemRepository.Setup(ir => ir.GetByCode(It.IsAny<int>())).Returns(this.item);
            this.itemRepository.Setup(ir => ir.Create(It.IsAny<Item>()));
            this.itemRepository.Setup(ir => ir.Update(It.IsAny<int>(), It.IsAny<Item>()));
            this.itemRepository.Setup(ir => ir.Delete(It.IsAny<int>()));

            this.imageService.Setup(s => s.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            this.imageService.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync("https://shorturl.at/fgwFK");
            this.imageService.Setup(s => s.UpdateAsync(It.IsAny<string>(), It.IsAny<IFormFile>()));
            this.imageService.Setup(s => s.DeleteAsync(It.IsAny<string>()));

            this.memoryCache.Setup(mc => mc.Set(It.IsAny<string>(), It.IsAny<object>()));
            this.memoryCache.Setup(mc => mc.Remove(It.IsAny<string>()));
            this.memoryCache.Setup(mc => mc.Clear());
            this.memoryCache.Setup(mc => mc.Remove(It.IsAny<string>));
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

            // Act
            ItemDetailsDto itemDetailsDto = this.itemService.GetByCode(ITEM_CODE);

            // Assert
            itemDetailsDto.Should().Be(this.itemDetailsDto);
        }

        [Test]
        public void GetItemByCode_Should_Return_ItemDetailsDto_Mapped_From_Cache()
        {
            // Arrange
            this.memoryCache.Setup(mc => mc.Get<ItemDetailsDto>(It.IsAny<string>())).Returns(this.itemDetailsDto);

            // Act
            ItemDetailsDto itemDetailsDto = this.itemService.GetByCode(ITEM_CODE);

            // Assert
            itemDetailsDto.Should().BeEquivalentTo(this.itemDetailsDto);
        }

        [Test]
        public async Task CreateAsyncItem_Should_Not_Throw_Exception()
        {
            // Arrange

            // Act
            Func<Task> task = async () => await this.itemService.CreateAsync(this.manageItemDto, null);

            // Assert
            await task.Should().NotThrowAsync();
        }

        [Test]
        public async Task UpdateAsyncItem_Should_Not_Throw_Exception()
        {
            // Arrange

            // Act
            Func<Task> task = async () => await this.itemService.UpdateAsync(ITEM_CODE, this.manageItemDto, null);

            // Assert
            await task.Should().NotThrowAsync();
        }

        [Test]
        public void DeleteItem_Should_Not_Throw_Exception()
        {
            // Arrange

            // Act
            Action action = () => this.itemService.Delete(ITEM_CODE);

            // Assert
            action.Should().NotThrow();
        }
    }
}