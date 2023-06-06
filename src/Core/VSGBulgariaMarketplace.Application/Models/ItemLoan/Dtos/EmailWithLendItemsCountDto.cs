namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos
{
    using System.Text.Json.Serialization;

    public class EmailWithLendItemsCountDto
    {
        public string Email { get; set; }

        [JsonPropertyName("count")]
        public int LendItemsCount { get; set; }
    }
}
