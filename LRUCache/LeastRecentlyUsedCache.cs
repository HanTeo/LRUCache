using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LRUCache
{
    public class LeastRecentlyUsedCache<TKey, TValue> : ILeastRecentlyUsedCache<TKey, TValue> where TKey : class
    {
        private readonly uint _capacity;
        private readonly ConcurrentDictionary<TKey, Node<TKey,TValue>> _entries;
        private Node<TKey, TValue> _head;
        private Node<TKey, TValue> _tail;

        public LeastRecentlyUsedCache(uint capacity)
        {
            _capacity = capacity;
            _entries = new ConcurrentDictionary<TKey, Node<TKey, TValue>>();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Node<TKey, TValue> node;
            if (_entries.TryGetValue(key, out node))
            {
                lock (node)
                {
                    UpdateRecentsList(node);
                    value = node.Value;
                }
                return true;
            }

            value = default(TValue);

            return false;
        }

        private void UpdateRecentsList(Node<TKey, TValue> node)
        {
            Remove(node);
            Prepend(node);
        }

        public void Set(TKey key, TValue value)
        {
            Node<TKey, TValue> node;
            if (_entries.TryGetValue(key, out node))
            {
                lock (node)
                {
                    node.Value = value;
                    UpdateRecentsList(node);
                }
            }
            else
            {
                lock (key)
                {
                    // If Eviction Necessary
                    if (_entries.Count >= _capacity)
                    {
                        Node<TKey, TValue> evicted;
                        if (_entries.TryRemove(_tail.Key, out evicted))
                        {
                            Remove(evicted);
                        }
                    }

                    var newNode = new Node<TKey, TValue>(key, value);

                    Prepend(newNode);

                    _entries.TryAdd(key, newNode);
                }
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

        public override string ToString()
        {
            var sb = new List<string>();
            var curr = _head;
            while (curr != null)
            {
                sb.Add(curr.Key.ToString());
                curr = curr.Next;
            }

            return string.Join(",",sb);
        }
    }
}