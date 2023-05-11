namespace VSGBulgariaMarketplace.Application.Services.HelpServices.Interfaces
{
    using Microsoft.Extensions.Caching.Memory;

    public interface IMemoryCacheAdapter : IMemoryCache
    {
        void Set(string key, object value);

        T Get<T>(string key);

        void Remove(string key);

        void Clear();
    }
}
