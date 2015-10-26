namespace LRUCache
{
    public interface ILeastRecentlyUsedCache<in TKey, TValue> where TKey : class
    {
        bool TryGetValue(TKey key, out TValue value);
        void Set(TKey key, TValue value);
    }
}