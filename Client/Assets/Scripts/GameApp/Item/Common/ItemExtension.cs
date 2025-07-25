using System;
using GameFramework.DataTable;
using UnityGameFramework.Runtime;

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
            IDataTable<DRUIItem> dtItem = GameEntry.DataTable.GetDataTable<DRUIItem>();
            DRUIItem drItem = dtItem.GetDataRow(uiItemID);

            if (drItem == null)
            {
                Log.Warning("Can not load item id '{0}' from data table.", drItem.Id.ToString());
                return;
            }

            itemComponent.ShowItem(serialId, logicType, drItem.AssetName, drItem.ItemGroupName, Constant.AssetPriority.Item_Asset, userData);
        }

        public static int GenerateSerialId(this ItemComponent itemComponent)
        {
            return ++s_SerialId;
        }
    }
}