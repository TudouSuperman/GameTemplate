using UnityEngine;
using GameApp.Singleton;

namespace GameApp
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : UnityEngine.MonoBehaviour
    {
        private void Start()
        {
            InitBuiltinComponents();
            InitCustomComponents();
        }

        private void Update()
        {
            SingletonManager.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
            SingletonManager.LateUpdate();
        }

        private void OnApplicationQuit()
        {
            SingletonManager.Clear();
            UnityGameFramework.Runtime.GameEntry.Shutdown(UnityGameFramework.Runtime.ShutdownType.Quit);
        }
    }
}