namespace VSGBulgariaMarketplace.Application.Models.Order.Dtos
{
    using System.Text.Json.Serialization;

    public class PendingOrderDto
    {
        public string Id { get; set; }

        public int ItemCode { get; set; }

        public short Quantity { get; set; }

        [JsonPropertyName("orderPrice")]
        public decimal Price { get; set; }

        public string OrderedBy { get; set; }

        public DateTime OrderDate { get; set; }
    }
}