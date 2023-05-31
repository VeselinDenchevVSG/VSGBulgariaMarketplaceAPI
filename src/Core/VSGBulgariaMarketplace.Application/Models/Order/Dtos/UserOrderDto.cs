namespace VSGBulgariaMarketplace.Application.Models.Order.Dtos
{
    using System.Text.Json.Serialization;

    public class UserOrderDto
    {
        [JsonPropertyName("code")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string ItemName { get; set; }

        public short Quantity { get; set; }

        [JsonPropertyName("orderPrice")]
        public decimal Price { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }
    }
}
