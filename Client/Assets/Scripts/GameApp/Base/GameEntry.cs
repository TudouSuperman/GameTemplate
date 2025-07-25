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

        private void OnApplicationQuit()
        {
            UnityGameFramework.Runtime.GameEntry.Shutdown(UnityGameFramework.Runtime.ShutdownType.Quit);
        }
    }
}