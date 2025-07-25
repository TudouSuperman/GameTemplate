using System;
using GameFramework;

namespace GameApp.Singleton
{
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        private bool m_Disposed;
        private static T s_Instance;
        public static T Instance => s_Instance;

        public void Register()
        {
            if (Instance != null)
            {
                throw new Exception(Utility.Text.Format("Singleton register twice! {0}", typeof(T).Name));
            }

            s_Instance = (T)this;
        }

        public void Destroy()
        {
            if (m_Disposed)
            {
                return;
            }

            m_Disposed = true;

            T _t = s_Instance;
            s_Instance = null;
            _t.Dispose();
        }

        public bool IsDispose() => m_Disposed;

        public virtual void Dispose()
        {
        }
    }
}