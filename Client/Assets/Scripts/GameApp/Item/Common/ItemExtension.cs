using System;
using GameFramework.DataTable;
using UnityGameFramework.Runtime;
using GameApp.DataTable;

namespace GameApp
{
    public static class ItemExtension
    {
        private static int s_SerialId = 0;

        public static void ShowItem(this ItemComponent itemComponent, int serialId, int itemId, object userData = null)
        {
            itemComponent.ShowItem(serialId, itemId, null, userData);
        }

        public static void ShowItem<T>(this ItemComponent itemComponent, int serialId, int itemId, object userData = null)
        {
            itemComponent.ShowItem(serialId, itemId, typeof(T), userData);
        }

        public static void ShowItem(this ItemComponent itemComponent, int serialId, int uiItemID, Type logicType, object userData = null)
        {
            DRUIItem drItem = GameEntry.DataTable.GetDataRow<DRUIItem>(uiItemID);
            if (drItem == null)
            {
                return;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drItem.AssetId);
            if (drAsset == null)
            {
                return;
            }

            DRSoundGroup drSoundGroup = GameEntry.DataTable.GetDataRow<DRSoundGroup>(drItem.ItemGroupId);
            if (drSoundGroup == null)
            {
                return;
            }

            itemComponent.ShowItem(serialId, logicType, drAsset.AssetPath, drSoundGroup.GroupName, Constant.AssetPriority.Item_Asset, userData);
        }

        public static int GenerateSerialId(this ItemComponent itemComponent)
        {
            return ++s_SerialId;
        }
    }
}