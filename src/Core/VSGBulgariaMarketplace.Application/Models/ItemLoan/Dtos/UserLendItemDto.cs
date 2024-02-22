namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos
{
    using System.Text.Json.Serialization;

    using static VSGBulgariaMarketplace.Application.Constants.JsonConstant;

    public class UserLendItemDto
    {
        public string Id { get; set; }

        public string ItemId { get; set; }

        [JsonPropertyName(LEND_ITEMS_EMAIL_JSON_PROPERTY_NAME)]
        public string Email { get; set; }

        public int Quantity { get; set; }

        [JsonPropertyName(USER_LEND_ITEMS_START_DATE_JSON_PROPERTY_NAME)]
        public DateTime StartDate { get; set; }

        [JsonPropertyName(USER_LEND_ITEMS_END_DATE_JSON_PROPERTY_NAME)]
        public DateTime? EndDate { get; set; }
    }
}
