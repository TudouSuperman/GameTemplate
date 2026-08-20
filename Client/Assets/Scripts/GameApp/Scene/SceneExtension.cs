using UnityGameFramework.Runtime;

namespace GameApp
{
    public static partial class SceneExtension
    {
        public static bool SceneIsLoading(this SceneComponent sceneComponent, int sceneId)
        {
            if (!TryGetTableData(sceneId, out DRScene drScene, out DRAsset drAsset))
            {
                return false;
            }

            return sceneComponent.SceneIsLoading(drAsset.AssetPath);
        }

        public static bool SceneIsLoaded(this SceneComponent sceneComponent, int sceneId)
        {
            if (!TryGetTableData(sceneId, out DRScene drScene, out DRAsset drAsset))
            {
                return false;
            }

            return sceneComponent.SceneIsLoaded(drAsset.AssetPath);
        }

        public static bool SceneIsUnloading(this SceneComponent sceneComponent, int sceneId)
        {
            if (!TryGetTableData(sceneId, out DRScene drScene, out DRAsset drAsset))
            {
                return false;
            }

            return sceneComponent.SceneIsUnloading(drAsset.AssetPath);
        }

        public static bool CanLoadScene(this SceneComponent sceneComponent, int sceneId)
        {
            if (!TryGetTableData(sceneId, out DRScene drScene, out DRAsset drAsset))
            {
                return false;
            }

            return !sceneComponent.SceneIsLoading(drAsset.AssetPath) && sceneComponent.SceneIsLoaded(drAsset.AssetPath) && !sceneComponent.SceneIsUnloading(drAsset.AssetPath);
        }

        public static void LoadScene(this SceneComponent sceneComponent, int sceneId, object userData = null)
        {
            if (!TryGetTableData(sceneId, out DRScene drScene, out DRAsset drAsset))
            {
                return;
            }

            sceneComponent.LoadScene(drAsset.AssetPath, Constant.AssetPriority.Scene_Asset, userData);
        }

        public static void UnloadScene(this SceneComponent sceneComponent, int sceneId, object userData = null)
        {
            if (!TryGetTableData(sceneId, out DRScene drScene, out DRAsset drAsset))
            {
                return;
            }

            sceneComponent.UnloadScene(drAsset.AssetPath, userData);
        }

        private static bool TryGetTableData(int sceneId, out DRScene drScene, out DRAsset drAsset)
        {
            drScene = null;
            drAsset = null;

            drScene = GameEntry.DataTable.GetDataRow<DRScene>(sceneId);
            if (drScene == null)
            {
                return false;
            }

            drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drScene.AssetId);
            if (drAsset == null)
            {
                return false;
            }

            return true;
        }
    }
}