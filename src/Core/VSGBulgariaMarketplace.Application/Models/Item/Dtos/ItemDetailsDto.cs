namespace VSGBulgariaMarketplace.Application.Models.Item.Dtos
{
    using System.Text.Json.Serialization;

    using static VSGBulgariaMarketplace.Application.Constants.JsonConstant;

    public class ItemDetailsDto
    {
        [JsonPropertyName(ITEM_IMAGE_URL_JSON_PROPERTY_NAME)]
        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public decimal? Price { get; set; }

        public string Category { get; set; }

        public short? QuantityForSale { get; set; }

        public string? Description { get; set; }
    }
}
