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

        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Artwork/Font/{0}.asset", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Scene/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Artwork/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Artwork/Sound/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Artwork/Entity/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Artwork/UI/UIForm/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Artwork/UI/UISound/{0}.wav", assetName);
        }

        public static string GetHotDictionaryAsset(string assetName, bool fromBytes = true)
        {
            return Utility.Text.Format("Assets/Res/Generate/GameHot/Localization/{0}.{1}", assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetHotGameAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/HotUpdate/{0}", assetName);
        }

        public static string GetHotTableDataAsset(string assetName, bool fromBytes = true)
        {
            return Utility.Text.Format("Assets/Res/Generate/GameHot/TableData/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }
    }
}