using System;
using System.Collections.Generic;

namespace GameApp
{
    public sealed class DoubleMap<K, V>
    {
        private readonly Dictionary<K, V> m_KeyValueDic = new();
        private readonly Dictionary<V, K> m_ValueKeyDic = new();

        public DoubleMap()
        {
        }

        public DoubleMap(int capacity)
        {
            m_KeyValueDic = new Dictionary<K, V>(capacity);
            m_ValueKeyDic = new Dictionary<V, K>(capacity);
        }

        public void ForEach(Action<K, V> action)
        {
            if (action == null)
            {
                return;
            }

            Dictionary<K, V>.KeyCollection keys = m_KeyValueDic.Keys;
            foreach (K key in keys)
            {
                action(key, m_KeyValueDic[key]);
            }
        }

        public List<K> Keys
        {
            get
            {
                return new List<K>(m_KeyValueDic.Keys);
            }
        }

        public List<V> Values
        {
            get
            {
                return new List<V>(m_ValueKeyDic.Keys);
            }
        }

        public void Add(K key, V value)
        {
            if (key == null || value == null || m_KeyValueDic.ContainsKey(key) || m_ValueKeyDic.ContainsKey(value))
            {
                return;
            }

            m_KeyValueDic.Add(key, value);
            m_ValueKeyDic.Add(value, key);
        }

        public V GetValueByKey(K key)
        {
            if (key != null && m_KeyValueDic.ContainsKey(key))
            {
                return m_KeyValueDic[key];
            }

            return default(V);
        }

        public K GetKeyByValue(V value)
        {
            if (value != null && m_ValueKeyDic.ContainsKey(value))
            {
                return m_ValueKeyDic[value];
            }

            return default(K);
        }

        public void RemoveByKey(K key)
        {
            if (key == null)
            {
                return;
            }

            V value;
            if (!m_KeyValueDic.TryGetValue(key, out value))
            {
                return;
            }

            m_KeyValueDic.Remove(key);
            m_ValueKeyDic.Remove(value);
        }

        public void RemoveByValue(V value)
        {
            if (value == null)
            {
                return;
            }

            K key;
            if (!m_ValueKeyDic.TryGetValue(value, out key))
            {
                return;
            }

            m_KeyValueDic.Remove(key);
            m_ValueKeyDic.Remove(value);
        }

        public void Clear()
        {
            m_KeyValueDic.Clear();
            m_ValueKeyDic.Clear();
        }

        public bool ContainsKey(K key)
        {
            if (key == null)
            {
                return false;
            }

            return m_KeyValueDic.ContainsKey(key);
        }

        public bool ContainsValue(V value)
        {
            if (value == null)
            {
                return false;
            }

            return m_ValueKeyDic.ContainsKey(value);
        }

        public bool Contains(K key, V value)
        {
            if (key == null || value == null)
            {
                return false;
            }

            return m_KeyValueDic.ContainsKey(key) && m_ValueKeyDic.ContainsKey(value);
        }
    }
}