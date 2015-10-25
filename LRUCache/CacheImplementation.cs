using System;

namespace LRUCache
{
    public class CacheImplementation : IProvider, IUpdateCallback
    {
        private IProvider _provider;
        private IUpdater _updater;

        private LeastRecentlyUsedCache<string, string> _cache; 

        public CacheImplementation(IProvider provider, IUpdater updater, uint capacity=10)
        {
            _provider = provider;
            _updater = updater;

            _cache = new LeastRecentlyUsedCache<string, string>(capacity);      
        }
        
        public string GetData(string key)
        {
            string value;
            if (_cache.TryGetValue(key, out value))
            {
                return value;
            }

            _updater.AddSubscription(key);

            return _provider.GetData(key);
        }

        public void Updated(string key, string newValue)
        {
            _cache.Set(key, newValue);
        }
    }
}
