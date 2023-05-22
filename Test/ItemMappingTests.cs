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
            private const int ITEM_CODE = 1;
            private const string ITEM_NAME = "Test";
            private const string ITEM_IMAGE_PUBLIC_ID = "Test";
            private const string ITEM_IMAGE_URL = "https://shorturl.at/fgwFK";
            private const decimal ITEM_PRICE = 1.11m;
            private const short ITEM_QUANTITY_COMBINED = 1;
            private const short ITEM_QUANTITY_FOR_SALE = 1;
            private const Category ITEM_CATEGORY = Category.Laptops;
            private const string ITEM_DESCRIPTION = "Test";

            private readonly IMapper mapper;
            private readonly Item item;

            public ItemMappingTests()
            {
                MapperConfiguration configuration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ItemProfile>();
                });

                this.mapper = new Mapper(configuration);

                this.item = new Item()
                {
                    Id = ITEM_CODE,
                    Name = ITEM_NAME,
                    Image = new CloudinaryImage()
                    {
                        Id = ITEM_IMAGE_PUBLIC_ID,
                        SecureUrl = ITEM_IMAGE_URL
                    },
                    ImagePublicId = ITEM_IMAGE_PUBLIC_ID,
                    Price = ITEM_PRICE,
                    Category = ITEM_CATEGORY,
                    QuantityCombined = ITEM_QUANTITY_COMBINED
                };
            }

            [Test]
            public void Item_Id_Should_Map_To_Item_Dtos_Code()
            {
                MarketplaceItemDto marketplaceItemDto = this.mapper.Map<Item, MarketplaceItemDto>(this.item);
                ManageItemDto manageItemDto = this.mapper.Map<Item, ManageItemDto>(this.item);
                InventoryItemDto inventoryItemDto = this.mapper.Map<Item, InventoryItemDto>(this.item);

                using (new AssertionScope())
                {
                    marketplaceItemDto.Code.Should().Be(this.item.Id);
                    manageItemDto.Code.Should().Be(this.item.Id);
                    inventoryItemDto.Code.Should().Be(this.item.Id);
                }
            }

            [Test]
            public void Item_With_QuantityForSale_Equal_To_Null_Should_Map_To_Zero_In_InventoryItemDto()
            {
                InventoryItemDto itemDto = this.mapper.Map<Item, InventoryItemDto>(this.item);

                itemDto.QuantityForSale.Should().Be(0);
            }

            [Test]
            public void Item_Image_SecureUrl_Should_Map_To_Item_Dtos_ImageUrl()
            {
                MarketplaceItemDto marketplaceItemDto = this.mapper.Map<Item, MarketplaceItemDto>(this.item);
                ItemDetailsDto itemDetailsDto = this.mapper.Map<Item, ItemDetailsDto>(this.item);

                using (new AssertionScope())
                {
                    marketplaceItemDto.ImageUrl?.Should().Be(this.item.Image?.SecureUrl);
                    itemDetailsDto.ImageUrl?.Should().Be(this.item.Image?.SecureUrl);
                }
            }
        }
    }
}