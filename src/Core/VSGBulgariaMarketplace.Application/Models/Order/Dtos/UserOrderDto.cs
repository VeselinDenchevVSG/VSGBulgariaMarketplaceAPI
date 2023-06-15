namespace VSGBulgariaMarketplace.Application.Models.Order.Dtos
{
    using System.Text.Json.Serialization;

    using VSGBulgariaMarketplace.Application.Constants;

    public class UserOrderDto
    {
        public string Id { get; set; }

        public string ItemCode { get; set; }

        [JsonPropertyName(JsonConstant.USER_ORDER_ITEM_NAME_JSON_PROPERTY_NAME)]
        public string ItemName { get; set; }

        public short Quantity { get; set; }

        [JsonPropertyName(JsonConstant.PENDING_ORDER_PRICE_JSON_PROPERTY_NAME)]
        public decimal Price { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }
    }
}
