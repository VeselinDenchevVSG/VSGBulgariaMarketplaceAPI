namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos
{
    using System.Text.Json.Serialization;

    using VSGBulgariaMarketplace.Application.Constants;

    public class EmailWithLendItemsCountDto
    {
        public string Email { get; set; }

        [JsonPropertyName(JsonConstant.EMAIL_WITH_LEND_ITEMS_COUNT_LEND_ITEMS_COUNT_JSON_PROPERTY_NAME)]
        public int LendItemsCount { get; set; }
    }
}
