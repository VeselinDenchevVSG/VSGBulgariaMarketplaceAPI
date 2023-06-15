namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos
{
    using System.Text.Json.Serialization;

    using VSGBulgariaMarketplace.Application.Constants;

    public class LendItemsDto
    {
        public short Quantity { get; set; }

        [JsonPropertyName(JsonConstant.LEND_ITEMS_EMAIL_JSON_PROPERTY_NAME)]
        public string Email { get; set; }
    }
}
