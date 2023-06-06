namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos
{
    using System.Text.Json.Serialization;

    public class LendItemsDto
    {
        //public string ItemId { get; set; }

        public short Quantity { get; set; }

        [JsonPropertyName("orderedBy")]
        public string Email { get; set; }
    }
}
