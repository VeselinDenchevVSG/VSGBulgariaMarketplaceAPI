namespace VSGBulgariaMarketplace.Domain.Entities
{
    using VSGBulgariaMarketplace.Domain.Interfaces;

    public abstract class BaseEntity<T> : IIdentity<T>, IAuditable
    {
        public T Id { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime ModifiedAtUtc { get; set; }
    }
}