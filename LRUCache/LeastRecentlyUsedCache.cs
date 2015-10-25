using System;
using System.Collections.Concurrent;

namespace LRUCache
{
    public class LeastRecentlyUsedCache<TKey, TValue>
    {
        private readonly uint _capacity;
        private readonly ConcurrentDictionary<TKey, Node<TKey,TValue>> _entries;
        private readonly ConcurrentQueue<TKey> _recentsList; 
        private Node<TKey, TValue> _head;
        private Node<TKey, TValue> _tail;

        static LeastRecentlyUsedCache()
        {
            var defVal = default(TValue);
            if (defVal is ValueType && Nullable.GetUnderlyingType(typeof(TValue)) == null)
            {
                throw new TypeLoadException(
                    $"TValue must be a Reference Type or a Nullable Type: {typeof (TValue)}");
            }
        }

        public LeastRecentlyUsedCache(uint capacity)
        {
            _capacity = capacity;
            _entries = new ConcurrentDictionary<TKey, Node<TKey, TValue>>();
            _recentsList = new ConcurrentQueue<TKey>();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Node<TKey, TValue> node;
            if (IsCacheHit(key, out node))
            {
                Reorder(node);

                value = node.Value;

                return true;
            }

            value = default(TValue);

            return false;
        }

        private void Reorder(Node<TKey, TValue> node)
        {
            Remove(node);
            Prepend(node);
        }

        public void Set(TKey key, TValue value)
        {
            Node<TKey, TValue> node;
            if (IsCacheHit(key, out node))
            {
                node.Value = value;

                Reorder(node);
            }
            else
            {
                if (IsFullCapacity())
                {
                    Evict();
                }

                var newNode = new Node<TKey, TValue>(key, value);

                Prepend(newNode);

                _entries.TryAdd(key, newNode);
            }
        }

        private void Prepend(Node<TKey, TValue> node)
        {
            node.Next = _head;
            node.Prev = null;

            if (_head != null)
            {
                _head.Prev = node;
            }

            _head = node;

            if (_tail == null)
            {
                _tail = _head;
            }
        }

        private void Remove(Node<TKey, TValue> node)
        {
            if (node.Prev == null)
            {
                _head = node.Next;
            }
            else
            {
                node.Prev.Next = node.Next;
            }

            if (node.Next == null)
            {
                _tail = node.Prev;
            }
            else
            {
                node.Next.Prev = node.Prev;
            }
        }

        private bool IsCacheHit(TKey key, out Node<TKey, TValue> node)
        {
            return _entries.TryGetValue(key, out node);
        }

        private void Evict()
        {
            Node<TKey, TValue> evicted;
            if (_entries.TryRemove(_tail.Key, out evicted))
            {
                Remove(evicted);
            }
        }

        private bool IsFullCapacity()
        {
            return _entries.Count >= _capacity;
        }
    }
}