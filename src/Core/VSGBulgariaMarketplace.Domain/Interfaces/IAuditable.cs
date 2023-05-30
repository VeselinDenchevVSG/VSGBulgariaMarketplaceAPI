namespace VSGBulgariaMarketplace.Domain.Interfaces
{
    public interface IAuditable
    {
        public DateTime CreatedAtUtc { get; set; }

        public DateTime ModifiedAtUtc { get; set; }
    }
}
