namespace VSGBulgariaMarketplace.Application.Services.HelpServices.Interfaces
{
    public interface ICacheService
    {
        void Set(string key, object value);

        T Get<T>(string key);

        void Remove(string key);

        void Clear();
    }
}
