namespace Test
{
    namespace Test
    {
        using AutoMapper;

        using FluentAssertions;
        using FluentAssertions.Execution;

        using NUnit.Framework;

        using VSGBulgariaMarketplace.Application.Helpers.MapProfiles;
        using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
        using VSGBulgariaMarketplace.Domain.Entities;
        using VSGBulgariaMarketplace.Domain.Enums;

        public class ItemMappingTests
        {
            private readonly IMapper mapper;

            private Item item;

            public ItemMappingTests()
            {
                MapperConfiguration configuration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ItemProfile>();
                });

                this.mapper = new Mapper(configuration);
            }

            [SetUp]
            public void Setup()
            {
                item = new Item()
                {
                    Id = 1,
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
            }

            [Test]
            public void Item_Id_Should_Map_To_Item_Dtos_Code()
            {
                MarketplaceItemDto marketplaceItemDto = this.mapper.Map<Item, MarketplaceItemDto>(item);
                ManageItemDto manageItemDto = this.mapper.Map<Item, ManageItemDto>(item);
                InventoryItemDto inventoryItemDto = this.mapper.Map<Item, InventoryItemDto>(item);

                using (new AssertionScope())
                {
                    marketplaceItemDto.Code.Should().Be(item.Id);
                    manageItemDto.Code.Should().Be(item.Id);
                    inventoryItemDto.Code.Should().Be(item.Id);
                }
            }

            [Test]
            public void Item_With_QuantityForSale_Equal_To_Null_Should_Map_To_Zero_In_InventoryItemDto()
            {
                InventoryItemDto itemDto = this.mapper.Map<Item, InventoryItemDto>(item);

                itemDto.QuantityForSale.Should().Be(0);
            }

            [Test]
            public void Item_Image_SecureUrl_Should_Map_To_Item_Dtos_ImageUrl()
            {
                MarketplaceItemDto marketplaceItemDto = this.mapper.Map<Item, MarketplaceItemDto>(item);
                ItemDetailsDto itemDetailsDto = this.mapper.Map<Item, ItemDetailsDto>(item);

                using (new AssertionScope())
                {
                    marketplaceItemDto.ImageUrl?.Should().Be(item.Image?.SecureUrl);
                    itemDetailsDto.ImageUrl?.Should().Be(item.Image?.SecureUrl);
                }
            }
        }
    }
}