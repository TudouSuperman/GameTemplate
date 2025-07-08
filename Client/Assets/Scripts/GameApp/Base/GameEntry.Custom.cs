using UnityGameFramework.Runtime;
using UnityGameFramework.Extensions;

namespace GameApp
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry
    {
        public static BuiltinDataComponent BuiltinData
        {
            get;
            private set;
        }
        
        public static ItemComponent Item
        {
            get;
            private set;
        }
        
        public static DataTableExtensionComponent DataTable2
        {
            get;
            private set;
        }
        
        public static CameraComponent Camera
        {
            get;
            private set;
        }
        
        public static PlatformComponent Platform
        {
            get;
            private set;
        }
        
        public static ScreenComponent Screen
        {
            get;
            private set;
        }

        private static void InitCustomComponents()
        {
            BuiltinData = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinDataComponent>();
            Item = UnityGameFramework.Runtime.GameEntry.GetComponent<ItemComponent>();
            DataTable2 = UnityGameFramework.Runtime.GameEntry.GetComponent<DataTableExtensionComponent>();
            Camera = UnityGameFramework.Runtime.GameEntry.GetComponent<CameraComponent>();
            Platform = UnityGameFramework.Runtime.GameEntry.GetComponent<PlatformComponent>();
            Screen = UnityGameFramework.Runtime.GameEntry.GetComponent<ScreenComponent>();
        }
    }
}