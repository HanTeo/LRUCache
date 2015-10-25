namespace LRUCache
{
    public interface IUpdateCallback
    {
        void Updated(string key, string newValue);
    }
}