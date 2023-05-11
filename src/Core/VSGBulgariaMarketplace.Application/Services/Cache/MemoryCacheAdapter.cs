namespace VSGBulgariaMarketplace.Application.Services.HelpServices
{
    using Microsoft.Extensions.Caching.Memory;

    using VSGBulgariaMarketplace.Application.Services.HelpServices.Interfaces;

    public class MemoryCacheAdapter : IMemoryCacheAdapter
    {
        private IMemoryCache memoryCache;
        private HashSet<object> keys;

        public MemoryCacheAdapter(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.keys = new HashSet<object>();
        }

        public void Set(string key, object value)
        {
            this.memoryCache.Set(key, value);
            this.keys.Add(key);
        }

        public T Get<T>(string key) => this.memoryCache.Get<T>(key);

        public void Remove(string key)
        {
            this.memoryCache.Remove(key);
            this.keys.Remove(key);
        }

        public void Clear()
        {
            foreach (var key in this.keys)
            {
                this.memoryCache.Remove(key);
            }

            this.keys.Clear();
        }

        public bool TryGetValue(object key, out object value) => this.memoryCache.TryGetValue(key, out value);

        public ICacheEntry CreateEntry(object key)
        {
            this.keys.Add(key);
            
            return this.memoryCache.CreateEntry(key);
        }

        public void Remove(object key)
        {
            this.memoryCache.Remove(key);
            this.keys.Remove(key);
        }

        public void Dispose() => this.memoryCache.Dispose();
    }
}