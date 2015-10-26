namespace LRUCache
{
    public interface ILeastRecentlyUsedCache<TKey, TValue> where TKey : class
    {
        bool TryGetValue(TKey key, out TValue value);
        void Set(TKey key, TValue value);
    }
}