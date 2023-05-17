namespace VSGBulgariaMarketplace.Domain.Entities
{
    public class CloudinaryImage : BaseEntity<string>
    {
        public string SecureUrl { get; init; }
    }
}