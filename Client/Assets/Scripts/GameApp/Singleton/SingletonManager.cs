using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace GameApp.Singleton
{
    /// <summary>
    /// 单例类管理器。
    /// </summary>
    public static class SingletonManager
    {
        private static readonly Dictionary<Type, ISingleton> m_SingletonTypes = new();
        private static readonly Stack<ISingleton> m_Singletons = new();
        private static readonly Queue<ISingleton> m_Updates = new();
        private static readonly Queue<ISingleton> m_LateUpdates = new();

        /// <summary>
        /// 添加单例。
        /// </summary>
        public static T AddSingleton<T>() where T : Singleton<T>, new()
        {
            T _singleton = new T();
            AddSingleton(_singleton);
            return _singleton;
        }

        /// <summary>
        /// 添加单例。
        /// </summary>
        public static void AddSingleton(ISingleton singleton)
        {
            Type _singletonType = singleton.GetType();
            if (!m_SingletonTypes.TryAdd(_singletonType, singleton))
            {
                throw new Exception($"already exist singleton: {_singletonType.Name}");
            }

            m_Singletons.Push(singleton);
            singleton.Register();
            switch (singleton)
            {
                case ISingletonUpdate:
                    m_Updates.Enqueue(singleton);
                    break;
                case ISingletonLateUpdate:
                    m_LateUpdates.Enqueue(singleton);
                    break;
            }
        }

        /// <summary>
        /// 单例轮询。
        /// </summary>
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            int _count = m_Updates.Count;
            while (_count-- > 0)
            {
                ISingleton _singleton = m_Updates.Dequeue();
                if (_singleton.IsDispose() || _singleton is not ISingletonUpdate _update)
                {
                    continue;
                }

                m_Updates.Enqueue(_singleton);
                try
                {
                    _update.Update(elapseSeconds, realElapseSeconds);
                }
                catch (Exception e)
                {
                    Log.Fatal(e);
                }
            }
        }

        /// <summary>
        /// Late 单例轮询。
        /// </summary>
        public static void LateUpdate()
        {
            int _count = m_LateUpdates.Count;
            while (_count-- > 0)
            {
                ISingleton _singleton = m_LateUpdates.Dequeue();
                if (_singleton.IsDispose() || _singleton is not ISingletonLateUpdate _lateUpdate)
                {
                    continue;
                }

                m_LateUpdates.Enqueue(_singleton);
                try
                {
                    _lateUpdate.LateUpdate();
                }
                catch (Exception e)
                {
                    Log.Fatal(e);
                }
            }
        }

        /// <summary>
        /// 清理所有单例。
        /// </summary>
        public static void Clear()
        {
            // 顺序反过来清理。
            while (m_Singletons.Count > 0)
            {
                ISingleton _singleton = m_Singletons.Pop();
                _singleton.Destroy();
            }

            m_SingletonTypes.Clear();
        }
    }
}