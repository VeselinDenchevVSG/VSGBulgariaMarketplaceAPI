using VSGBulgariaMarketplace.Domain.Entities;
using VSGBulgariaMarketplace.Domain.Enums;

namespace Test.IntegrationTests.Persistence.Factories;

internal static class ItemFactory
{
    internal static Item GetItemWithNullProperties()
        => new()
            {
                Code = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                ImagePublicId = null,
                Price = null,
                Category = Category.Misc,
                QuantityCombined = 1,
                QuantityForSale = null,
                AvailableQuantity = null,
                Description = null,
                Location = Location.Home
            };

    internal static Item GetItemWithNotNullProperties(string imagePublicId)
        => new()
            {
                Code = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                ImagePublicId = imagePublicId,
                Price = 123.45m,
                Category = Category.Misc,
                QuantityCombined = 3,
                QuantityForSale = 1,
                AvailableQuantity = 1,
                Description = "test",
                Location = Location.Home
            };
}