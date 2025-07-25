namespace GameApp
{
    public static partial class Constant
    {
        /// <summary>
        /// 资源优先级（数值大的优先加载）。
        /// </summary>
        public static class AssetPriority
        {
            public const int Config_Asset = 100;
            public const int DataTable_Asset = 100;
            public const int Dictionary_Asset = 100;
            
            public const int Font_Asset = 90;
            
            public const int Item_Asset = 60;
            public const int Entity_Asset = 60;
            public const int Prefab_Asset = 60;
            public const int UIForm_Asset = 50;
            
            public const int UISound_Asset = 15;
            public const int Sound_Asset = 15;
            public const int Music_Asset = 10;
            
            public const int Scene_Asset = 0;
        }
    }
}