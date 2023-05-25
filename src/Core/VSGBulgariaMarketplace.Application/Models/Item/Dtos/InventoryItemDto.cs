namespace VSGBulgariaMarketplace.Application.Models.Item.Dtos
{
    using System.Text.Json.Serialization;

    public class InventoryItemDto
    {
        public int Code { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public short? QuantityForSale { get; set; }

        [JsonPropertyName("quantity")]
        public short QuantityCombined { get; set; }
    }
}
