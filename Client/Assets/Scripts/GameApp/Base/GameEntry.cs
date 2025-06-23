using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        private void Start()
        {
            InitBuiltinComponents();
            InitCustomComponents();
        }
        
        private void OnApplicationQuit()
        {
            GameApp.UI.ModelManager.Instance.Clear();
            UnityGameFramework.Runtime.GameEntry.Shutdown(UnityGameFramework.Runtime.ShutdownType.Quit);
        }
    }
}
