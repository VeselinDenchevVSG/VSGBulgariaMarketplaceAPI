namespace VSGBulgariaMarketplace.Application.Services.HelpServices
{
    using Microsoft.Extensions.Caching.Memory;

    using VSGBulgariaMarketplace.Application.Services.HelpServices.Interfaces;

    public class CacheService : ICacheService
    {
        private IMemoryCache cache;
        private HashSet<string> keys;

        public CacheService()
        {
            this.cache = new MemoryCache(new MemoryCacheOptions());
            this.keys = new HashSet<string>();
        }

        public void Set(string key, object value)
        {
            this.cache.Set(key, value);
            this.keys.Add(key);
        }

        public T Get<T>(string key) => this.cache.Get<T>(key);

        public void Remove(string key)
        {
            this.cache.Remove(key);
            this.keys.Remove(key);
        }

        public void Clear()
        {
            foreach (var key in this.keys)
            {
                this.cache.Remove(key);
            }

            this.keys.Clear();
        }
    }
}