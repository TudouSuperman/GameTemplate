using System;

namespace GameApp
{
    public abstract class Singleton<T> where T : class
    {
        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (null != m_Instance)
                {
                    return m_Instance;
                }

                // 通过反射创建实例（即使构造函数是 non-public）。
                m_Instance = Activator.CreateInstance(typeof(T), true) as T;
                if (null == m_Instance) throw new Exception($"单例：{typeof(T).Name} 构造失败！");
                return m_Instance;
            }
        }

        public abstract void Clear();
    }
}