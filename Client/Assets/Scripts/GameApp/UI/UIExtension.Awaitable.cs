using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;
using UnityGameFramework.Extension;

namespace GameApp
{
    public static partial class UIExtension
    {
        public static async UniTask<UIForm> OpenUIFormAsync(this UIComponent uiComponent,
            int uiFormId,
            object userData = null,
            CancellationToken cancellationToken = default,
            Action<float> updateEvent = null,
            Action<string> dependencyAssetEvent = null)
        {
            DRUIForm drUIForm = GameEntry.DataTable.GetDataRow<DRUIForm>(uiFormId);
            if (drUIForm == null)
            {
                return null;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drUIForm.AssetId);
            if (drAsset == null)
            {
                return null;
            }

            string assetName = drAsset.AssetPath;
            if (!drUIForm.AllowMultiInstance)
            {
                if (uiComponent.IsLoadingUIForm(assetName))
                {
                    return null;
                }

                if (uiComponent.HasUIForm(assetName))
                {
                    return null;
                }
            }
            
            return await uiComponent.OpenUIFormAsync(
                assetName,
                drUIForm.GroupName,
                Constant.AssetPriority.UIForm_Asset,
                drUIForm.PauseCoveredUIForm,
                userData,
                cancellationToken,
                updateEvent,
                dependencyAssetEvent);
        }
    }
}