﻿namespace VSGBulgariaMarketplace.Application.Models.Order.Dtos
{
    using System.Text.Json.Serialization;

    using static VSGBulgariaMarketplace.Application.Constants.JsonConstant;

    public class PendingOrderDto
    {
        public string Id { get; set; }

        public string ItemCode { get; set; }

        public short Quantity { get; set; }

        [JsonPropertyName(PENDING_ORDER_PRICE_JSON_PROPERTY_NAME)]
        public decimal Price { get; set; }

        public string OrderedBy { get; set; }

        public DateTime OrderDate { get; set; }
    }
}