namespace VSGBulgariaMarketplace.Domain.Entities
{
    using VSGBulgariaMarketplace.Domain.Interfaces;

    public abstract class BaseEntity : IIdentity<int>, IAuditable
    {
        public int Id { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime ModifiedAtUtc { get; set; }

        public DateTime? DeletedAtUtc { get; set; }

        public bool IsDeleted { get; set; }
    }
}
