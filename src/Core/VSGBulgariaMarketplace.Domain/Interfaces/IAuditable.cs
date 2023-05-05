namespace VSGBulgariaMarketplace.Domain.Interfaces
{
    public interface IAuditable
    {
        public DateTime CreatedAtUtc { get; set; }

        public DateTime ModifiedAtUtc { get; set; }

        public DateTime? DeletedAtUtc { get; set; }

        public bool IsDeleted { get; set; }
    }
}
