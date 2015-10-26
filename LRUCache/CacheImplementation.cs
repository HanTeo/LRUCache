namespace LRUCache
{
    public class CacheImplementation : IProvider, IUpdateCallback
    {
        private readonly IProvider _provider;
        private readonly IUpdater _updater;

        private readonly LeastRecentlyUsedCache<string, string> _cache; 

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

        public override string ToString()
        {
            return _cache.ToString();
        }
    }
}
