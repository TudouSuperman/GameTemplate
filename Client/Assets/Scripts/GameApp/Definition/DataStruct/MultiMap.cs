using System;
using System.Collections.Generic;

namespace GameApp
{
    public sealed class MultiMap<T, K> : SortedDictionary<T, List<K>>
    {
        private readonly List<K> m_Empty = new();
        private readonly int m_MaxPoolCount;
        private readonly Queue<List<K>> m_Pool;

        public MultiMap(int maxPoolCount = 0)
        {
            this.m_MaxPoolCount = maxPoolCount;
            this.m_Pool = new Queue<List<K>>(maxPoolCount);
        }

        private List<K> FetchList()
        {
            if (this.m_Pool.Count > 0)
            {
                return this.m_Pool.Dequeue();
            }

            return new List<K>(10);
        }

        private void Recycle(List<K> list)
        {
            if (list == null)
            {
                return;
            }

            if (this.m_Pool.Count == this.m_MaxPoolCount)
            {
                return;
            }

            list.Clear();
            this.m_Pool.Enqueue(list);
        }

        public void Add(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                list = this.FetchList();
                this.Add(t, list);
            }

            list.Add(k);
        }

        public bool Remove(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }

            if (!list.Remove(k))
            {
                return false;
            }

            if (list.Count == 0)
            {
                this.Remove(t);
            }

            return true;
        }

        public new bool Remove(T t)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }

            this.Recycle(list);
            return base.Remove(t);
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public K[] GetAll(T t)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return Array.Empty<K>();
            }

            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public new List<K> this[T t]
        {
            get
            {
                this.TryGetValue(t, out List<K> list);
                return list ?? m_Empty;
            }
        }

        public K GetOne(T t)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }

            return default;
        }

        public bool Contains(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }

            return list.Contains(k);
        }
    }
}