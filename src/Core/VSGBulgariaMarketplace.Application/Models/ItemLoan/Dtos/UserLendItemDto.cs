namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos
{
    using System.Text.Json.Serialization;

    public class UserLendItemDto
    {
        public string Id { get; set; }

        public string ItemId { get; set; }

        [JsonPropertyName("orderedBy")]
        public string Email { get; set; }

        public int Quantity { get; set; }

        [JsonPropertyName("loanStartDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("loanEndDate")]
        public DateTime? EndDate { get; set; }
    }
}
