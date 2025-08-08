using GameFramework;
using UnityGameFramework.Runtime;

namespace GameApp
{
    public static partial class SceneExtension
    {
        public static bool SceneIsLoading(this SceneComponent sceneComponent, int sceneId)
        {
            DRScene drScene = GameEntry.DataTable.GetDataRow<DRScene>(sceneId);
            if (drScene == null)
            {
                return false;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drScene.AssetId);
            if (drAsset == null)
            {
                return false;
            }

            return sceneComponent.SceneIsLoading(drAsset.AssetPath);
        }

        public static bool SceneIsLoaded(this SceneComponent sceneComponent, int sceneId)
        {
            DRScene drScene = GameEntry.DataTable.GetDataRow<DRScene>(sceneId);
            if (drScene == null)
            {
                return false;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drScene.AssetId);
            if (drAsset == null)
            {
                return false;
            }

            return sceneComponent.SceneIsLoaded(drAsset.AssetPath);
        }

        public static bool CanLoadScene(this SceneComponent sceneComponent, int sceneId)
        {
            DRScene drScene = GameEntry.DataTable.GetDataRow<DRScene>(sceneId);
            if (drScene == null)
            {
                return false;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drScene.AssetId);
            if (drAsset == null)
            {
                return false;
            }

            return !sceneComponent.SceneIsLoading(drAsset.AssetPath) && sceneComponent.SceneIsLoaded(drAsset.AssetPath);
        }

        public static void LoadScene(this SceneComponent sceneComponent, int sceneId, object userData = null)
        {
            DRScene drScene = GameEntry.DataTable.GetDataRow<DRScene>(sceneId);
            if (drScene == null)
            {
                return;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drScene.AssetId);
            if (drAsset == null)
            {
                return;
            }

            sceneComponent.LoadScene(drAsset.AssetPath, Constant.AssetPriority.Scene_Asset, userData);
        }

        public static void UnloadScene(this SceneComponent sceneComponent, int sceneId, object userData = null)
        {
            DRScene drScene = GameEntry.DataTable.GetDataRow<DRScene>(sceneId);
            if (drScene == null)
            {
                return;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drScene.AssetId);
            if (drAsset == null)
            {
                return;
            }

            sceneComponent.UnloadScene(drAsset.AssetPath, userData);
        }
    }
}