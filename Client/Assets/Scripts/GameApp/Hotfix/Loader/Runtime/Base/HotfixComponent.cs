using UnityEngine;

namespace GameApp.Hotfix
{
    public abstract class HotfixComponent : MonoBehaviour
    {
        /// <summary>
        /// 运行优先级，越大运行越靠前
        /// </summary>
        protected internal virtual int Priority => 0;

        protected internal abstract void OnInitialize();
        protected internal abstract void OnUpdate(float elapseSeconds, float realElapseSeconds);
        protected internal abstract void OnShutdown();

        protected virtual void Awake()
        {
            HotfixComponentEntry.RegisterComponent(this);
        }
    }
}
