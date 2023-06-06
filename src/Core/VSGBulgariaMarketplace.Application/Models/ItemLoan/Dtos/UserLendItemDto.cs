namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos
{
    public class UserLendItemDto
    {
        public string Id { get; set; }

        public string ItemId { get; set; }

        public string Email { get; set; }

        public int Quantity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
