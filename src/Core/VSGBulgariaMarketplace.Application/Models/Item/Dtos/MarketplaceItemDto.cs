namespace VSGBulgariaMarketplace.Application.Models.Item.Dtos
{
    using System.Text.Json.Serialization;

    public class MarketplaceItemDto
    {
        public int Code { get; set; }

        [JsonPropertyName("imageURL")]
        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public short? QuantityForSale { get; set; }
    }
}
