using GameFramework;

namespace GameApp.Hotfix
{
    public static class AssetHotfixPathUtility
    {
        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Artwork/Font/{0}.asset", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Scene/{0}.unity", assetName);
        }

        public static string GetDictionaryAsset(string assetName, bool fromBytes = true)
        {
            return Utility.Text.Format("Assets/Res/Generate/TableData/Localization/{0}.{1}", assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetHotfixTableDataAsset(string assetName, bool fromBytes = true)
        {
            return Utility.Text.Format("Assets/Res/Generate/TableData/GameHotfix/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }
    }
}