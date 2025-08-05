using GameFramework;

namespace GameApp
{
    public static class AssetHotPathUtility
    {
        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Artwork/Font/{0}.asset", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Scene/{0}.unity", assetName);
        }

        public static string GetHotDictionaryAsset(string assetName, bool fromBytes = true)
        {
            return Utility.Text.Format("Assets/Res/Generate/TableData/Localization/{0}.{1}", assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetHotTableDataAsset(string assetName, bool fromBytes = true)
        {
            return Utility.Text.Format("Assets/Res/Generate/TableData/GameHot/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }
    }
}