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
            return Utility.Text.Format("Assets/Res/Generate/TableData/Game/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDictionaryAsset(string assetName, bool fromBytes = true)
        {
            return Utility.Text.Format("Assets/Res/Generate/TableData/Localization/{0}.{1}", assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetHotfixGameAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Hotfix/{0}", assetName);
        }
    }
}