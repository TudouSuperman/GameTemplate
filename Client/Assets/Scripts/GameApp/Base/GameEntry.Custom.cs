using UnityGameFramework.Runtime;
using UnityGameFramework.Extension;

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
        
        public static CursorComponent Cursor
        {
            get;
            private set;
        }
        
        public static WebSocketComponent WebSocket
        {
            get;
            private set;
        }
        
        public static NetworkServiceComponent NetworkService
        {
            get;
            private set;
        }
        
        public static TimerComponent Timer
        {
            get;
            private set;
        }
        
        public static TimingWheelComponent TimingWheel
        {
            get;
            private set;
        }
        
        public static TextureSetComponent TextureSet
        {
            get;
            private set;
        }
        
        public static SpriteCollectionComponent SpriteCollection
        {
            get;
            private set;
        }
        
        public static CodeRunnerComponent CodeRunner
        {
            get;
            private set;
        }

        private static void InitCustomComponents()
        {
            BuiltinData = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinDataComponent>();
            Item = UnityGameFramework.Runtime.GameEntry.GetComponent<ItemComponent>();
            Camera = UnityGameFramework.Runtime.GameEntry.GetComponent<CameraComponent>();
            Platform = UnityGameFramework.Runtime.GameEntry.GetComponent<PlatformComponent>();
            Screen = UnityGameFramework.Runtime.GameEntry.GetComponent<ScreenComponent>();
            Cursor = UnityGameFramework.Runtime.GameEntry.GetComponent<CursorComponent>();
            WebSocket = UnityGameFramework.Runtime.GameEntry.GetComponent<WebSocketComponent>();
            NetworkService = UnityGameFramework.Runtime.GameEntry.GetComponent<NetworkServiceComponent>();
            Timer = UnityGameFramework.Runtime.GameEntry.GetComponent<TimerComponent>();
            TimingWheel = UnityGameFramework.Runtime.GameEntry.GetComponent<TimingWheelComponent>();
            TextureSet = UnityGameFramework.Runtime.GameEntry.GetComponent<TextureSetComponent>();
            SpriteCollection = UnityGameFramework.Runtime.GameEntry.GetComponent<SpriteCollectionComponent>();
            CodeRunner = UnityGameFramework.Runtime.GameEntry.GetComponent<CodeRunnerComponent>();
        }
    }
}