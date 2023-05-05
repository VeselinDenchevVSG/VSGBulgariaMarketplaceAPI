namespace VSGBulgariaMarketplace.Domain.Interfaces
{
    public interface IIdentity<T>
    {
        public T Id { get; set; }
    }
}
