namespace VSGBulgariaMarketplace.Application.Models.Item.Dtos
{
    using System.Text.Json.Serialization;

    using VSGBulgariaMarketplace.Application.Constants;

    public class InventoryItemDto
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public string Category { get; set; }

        public short? QuantityForSale { get; set; }

        public short? AvailableQuantity { get; set; }

        public decimal? Price { get; set; }

        [JsonPropertyName(JsonConstant.INVENTORY_ITEM_QUANTITY_COMBINED_JSON_PROPERTY_NAME)]
        public short QuantityCombined { get; set; }

        [JsonPropertyName(JsonConstant.ITEM_IMAGE_URL_JSON_PROPERTY_NAME)]
        public string? ImageUrl { get; set; }

        public string Location { get; set; }
    }
}
