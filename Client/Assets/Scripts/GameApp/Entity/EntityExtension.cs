using System;
using UnityGameFramework.Runtime;

namespace GameApp
{
    public static partial class EntityExtension
    {
        // 关于 EntityId 的约定：
        // 0 为无效
        // 正值用于和服务器通信的实体（如玩家角色、NPC、怪等，服务器只产生正值）
        // 负值用于本地生成的临时实体（如特效、FakeObject等）
        private static int s_SerialId = 0;

        public static void TryHideEntity(this EntityComponent entityComponent, int serialId)
        {
            if (entityComponent.IsLoadingEntity(serialId) || entityComponent.HasEntity(serialId))
            {
                entityComponent.HideEntity(serialId);
            }
        }

        public static bool TryGetEntity(this EntityComponent entityComponent, int serialId, out UGFEntityLogic entityLogic)
        {
            entityLogic = null;
            Entity entity = entityComponent.GetEntity(serialId);
            if (entity == null)
            {
                return false;
            }

            entityLogic = (UGFEntityLogic)entity.Logic;
            return true;
        }

        public static void ShowEntity<T>(this EntityComponent entityComponent, UGFEntityData entityData) where T : EntityLogic
        {
            entityComponent.ShowEntity(typeof(T), entityData);
        }

        public static void ShowEntity(this EntityComponent entityComponent, Type logicType, UGFEntityData entityData)
        {
            if (entityData == null)
            {
                Log.Warning("Data is invalid.");
                return;
            }

            DREntity drEntity = GameEntry.DataTable.GetDataRow<DREntity>(entityData.TypeId);
            if (drEntity == null)
            {
                return;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drEntity.AssetId);
            if (drAsset == null)
            {
                return;
            }

            entityComponent.ShowEntity(entityData.Id, logicType, drAsset.AssetPath, drEntity.GroupName, Constant.AssetPriority.Entity_Asset, entityData);
        }

        public static int? ShowEntity<T>(this EntityComponent entityComponent, int entityTypeId, object userData = null) where T : UGFEntityLogic
        {
            return entityComponent.ShowEntity(entityTypeId, typeof(T), userData);
        }

        public static int? ShowEntity(this EntityComponent entityComponent, int entityTypeId, Type logicType, object userData = null)
        {
            DREntity drEntity = GameEntry.DataTable.GetDataRow<DREntity>(entityTypeId);
            if (drEntity == null)
            {
                return null;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drEntity.AssetId);
            if (drAsset == null)
            {
                return null;
            }

            int entityId = entityComponent.GenerateSerialId();
            entityComponent.ShowEntity(entityId, logicType, drAsset.AssetPath, drEntity.GroupName, Constant.AssetPriority.Entity_Asset, userData);
            return entityId;
        }

        public static int GenerateSerialId(this EntityComponent entityComponent)
        {
            return --s_SerialId;
        }
    }
}