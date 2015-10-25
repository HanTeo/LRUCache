namespace LRUCache
{
    public interface IUpdater
    {
        void AddSubscription(string key);
        void Unsubscribe(string key);
    }
}