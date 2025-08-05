using GameFramework.UI;
using UnityGameFramework.Runtime;
using GameApp.DataTable;

namespace GameApp.UI
{
    public static class UIExtension
    {
        public static bool HasUIForm(this UIComponent uiComponent, int uiFormId)
        {
            DRUIForm drUIForm = GameEntry.DataTable.GetDataRow<DRUIForm>(uiFormId);
            if (drUIForm == null)
            {
                return false;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drUIForm.AssetId);
            if (drAsset == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(drUIForm.GroupName))
            {
                return uiComponent.HasUIForm(drAsset.AssetPath);
            }

            IUIGroup uiGroup = uiComponent.GetUIGroup(drUIForm.GroupName);
            if (uiGroup == null)
            {
                return false;
            }

            return uiGroup.HasUIForm(drAsset.AssetPath);
        }

        public static UGuiFormLogic GetUIForm(this UIComponent uiComponent, int uiFormId)
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

            UIForm uiForm = null;
            if (string.IsNullOrEmpty(drUIForm.GroupName))
            {
                uiForm = uiComponent.GetUIForm(drAsset.AssetPath);
                if (uiForm == null)
                {
                    return null;
                }

                return (UGuiFormLogic)uiForm.Logic;
            }

            IUIGroup uiGroup = uiComponent.GetUIGroup(drUIForm.GroupName);
            if (uiGroup == null)
            {
                return null;
            }

            uiForm = (UIForm)uiGroup.GetUIForm(drAsset.AssetPath);
            if (uiForm == null)
            {
                return null;
            }

            return (UGuiFormLogic)uiForm.Logic;
        }

        public static int? OpenUIForm(this UIComponent uiComponent, int uiFormId, object userData = null)
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

            return uiComponent.OpenUIForm(assetName, drUIForm.GroupName, Constant.AssetPriority.UIForm_Asset, drUIForm.PauseCoveredUIForm, userData);
        }
    }
}