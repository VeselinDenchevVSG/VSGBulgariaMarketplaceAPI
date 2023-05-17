namespace VSGBulgariaMarketplace.Application.Models.Item.Dtos
{
    using Newtonsoft.Json;

    public class MarketplaceItemDto
    {
        public int Code { get; set; }

        [JsonProperty("ImageURL")]
        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public short? QuantityForSale { get; set; }
    }
}
