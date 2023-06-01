namespace VSGBulgariaMarketplace.Application.Models.Order.Dtos
{
    using System.Text.Json.Serialization;

    public class UserOrderDto
    {
        public string Id { get; set; }

        public string ItemCode { get; set; }

        [JsonPropertyName("name")]
        public string ItemName { get; set; }

        public short Quantity { get; set; }

        [JsonPropertyName("orderPrice")]
        public decimal Price { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }
    }
}
