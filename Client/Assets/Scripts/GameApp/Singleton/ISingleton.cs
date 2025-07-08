using System;

namespace GameApp.Singleton
{
    public interface ISingleton : IDisposable
    {
        void Register();
        void Destroy();
        bool IsDispose();
    }
}