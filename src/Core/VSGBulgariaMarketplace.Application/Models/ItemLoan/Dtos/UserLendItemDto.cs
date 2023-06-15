namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos
{
    using System.Text.Json.Serialization;

    using VSGBulgariaMarketplace.Application.Constants;

    public class UserLendItemDto
    {
        public string Id { get; set; }

        public string ItemId { get; set; }

        [JsonPropertyName(JsonConstant.LEND_ITEMS_EMAIL_JSON_PROPERTY_NAME)]
        public string Email { get; set; }

        public int Quantity { get; set; }

        [JsonPropertyName(JsonConstant.USER_LEND_ITEMS_START_DATE_JSON_PROPERTY_NAME)]
        public DateTime StartDate { get; set; }

        [JsonPropertyName(JsonConstant.USER_LEND_ITEMS_END_DATE_JSON_PROPERTY_NAME)]
        public DateTime? EndDate { get; set; }
    }
}
