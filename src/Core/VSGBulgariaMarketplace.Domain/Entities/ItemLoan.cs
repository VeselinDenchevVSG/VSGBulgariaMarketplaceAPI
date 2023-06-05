namespace VSGBulgariaMarketplace.Domain.Entities
{
    public class ItemLoan : BaseEntity<string>
    {
        public string ItemId { get; set; }

        public string Email { get; set; }

        public short Quantity { get; set; }

        public DateTime? EndDatetimeUtc { get; set; }
    }
}
