namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos
{
    using System.Text.Json.Serialization;

    using static VSGBulgariaMarketplace.Application.Constants.JsonConstant;

    public class LendItemsDto
    {
        public short Quantity { get; set; }

        [JsonPropertyName(LEND_ITEMS_EMAIL_JSON_PROPERTY_NAME)]
        public string Email { get; set; }
    }
}
