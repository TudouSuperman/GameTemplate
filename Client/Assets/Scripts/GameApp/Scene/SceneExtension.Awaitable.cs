using Cysharp.Threading.Tasks;
using GameFramework;
using UnityGameFramework.Runtime;
using UnityGameFramework.Extension;

namespace GameApp
{
    public static partial class SceneExtension
    {
        public static UniTask LoadSceneAsync(this SceneComponent sceneComponent, int sceneId)
        {
            DRScene drScene = GameEntry.DataTable.GetDataRow<DRScene>(sceneId);
            if (drScene == null)
            {
                string error = Utility.Text.Format("Can not load Scene '{0}' from data table.", sceneId.ToString());
                return UniTask.FromException(new GameFrameworkException(error));
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drScene.AssetId);
            if (drAsset == null)
            {
                string error = Utility.Text.Format("Can not load Scene '{0}' from data table.", sceneId.ToString());
                return UniTask.FromException(new GameFrameworkException(error));
            }

            return sceneComponent.LoadSceneAsync(drAsset.AssetPath, Constant.AssetPriority.Scene_Asset);
        }

        public static UniTask UnloadSceneAsync(this SceneComponent sceneComponent, int sceneId)
        {
            DRScene drScene = GameEntry.DataTable.GetDataRow<DRScene>(sceneId);
            if (drScene == null)
            {
                string error = Utility.Text.Format("Can not load Scene '{0}' from data table.", sceneId.ToString());
                return UniTask.FromException(new GameFrameworkException(error));
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drScene.AssetId);
            if (drAsset == null)
            {
                string error = Utility.Text.Format("Can not load Scene '{0}' from data table.", sceneId.ToString());
                return UniTask.FromException(new GameFrameworkException(error));
            }

            return sceneComponent.UnloadSceneAsync(drAsset.AssetPath);
        }
    }
}