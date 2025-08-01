using GameFramework;

namespace GameApp
{
    public static class AssetPathUtility
    {
        public static string GetConfigAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/Res/Config/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetTableDataAsset(string assetName, bool fromBytes = true)
        {
            return Utility.Text.Format("Assets/Res/Generate/Game/TableData/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetHotDictionaryAsset(string assetName, bool fromBytes = true)
        {
            return Utility.Text.Format("Assets/Res/Generate/GameHot/Localization/{0}.{1}", assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetHotGameAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/HotUpdate/{0}", assetName);
        }
    }
}