using System;
using System.Threading;
using UnityGameFramework.Runtime;
using UnityGameFramework.Extension;
using Cysharp.Threading.Tasks;

namespace GameApp
{
    public static partial class EntityExtension
    {
        public static UniTask<Entity> ShowEntityAsync<T>
        (
            this EntityComponent entityComponent,
            UGFEntityData userData,
            CancellationToken cancellationToken = default,
            Action<float> updateEvent = null,
            Action<string> dependencyAssetEvent = null
        ) where T : UGFEntityLogic
        {
            return entityComponent.ShowEntityAsync
            (
                userData.SerialId,
                userData.TypeId,
                typeof(T),
                userData,
                cancellationToken,
                updateEvent,
                dependencyAssetEvent
            );
        }

        public static UniTask<Entity> ShowEntityAsync<T>
        (
            this EntityComponent entityComponent,
            int entityTypeId,
            object userData = null,
            CancellationToken cancellationToken = default,
            Action<float> updateEvent = null,
            Action<string> dependencyAssetEvent = null
        ) where T : EntityLogic
        {
            return entityComponent.ShowEntityAsync
            (
                entityComponent.GenerateSerialId(),
                entityTypeId,
                typeof(T),
                userData,
                cancellationToken,
                updateEvent,
                dependencyAssetEvent
            );
        }

        public static UniTask<Entity> ShowEntityAsync<T>
        (
            this EntityComponent entityComponent,
            int serialId,
            int entityTypeId,
            object userData = null,
            CancellationToken cancellationToken = default,
            Action<float> updateEvent = null,
            Action<string> dependencyAssetEvent = null
        ) where T : EntityLogic
        {
            return entityComponent.ShowEntityAsync
            (
                serialId,
                entityTypeId,
                typeof(T),
                userData,
                cancellationToken,
                updateEvent,
                dependencyAssetEvent
            );
        }

        public static UniTask<Entity> ShowEntityAsync
        (
            this EntityComponent entityComponent,
            int entityTypeId,
            Type logicType,
            object userData = null,
            CancellationToken cancellationToken = default,
            Action<float> updateEvent = null,
            Action<string> dependencyAssetEvent = null
        )
        {
            return entityComponent.ShowEntityAsync
            (
                entityComponent.GenerateSerialId(),
                entityTypeId,
                logicType,
                userData,
                cancellationToken,
                updateEvent,
                dependencyAssetEvent
            );
        }

        public static UniTask<Entity> ShowEntityAsync
        (
            this EntityComponent entityComponent,
            int serialId,
            int entityTypeId,
            Type logicType,
            object userData = null,
            CancellationToken cancellationToken = default,
            Action<float> updateEvent = null,
            Action<string> dependencyAssetEvent = null
        )
        {
            DREntity drEntity = GameEntry.DataTable.GetDataRow<DREntity>(entityTypeId);
            if (drEntity == null)
            {
                return UniTask.FromResult<Entity>(null);
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drEntity.AssetId);
            if (drAsset == null)
            {
                return UniTask.FromResult<Entity>(null);
            }

            return entityComponent.ShowEntityAsync
            (
                serialId,
                logicType,
                drAsset.AssetPath,
                drEntity.GroupName,
                Constant.AssetPriority.Entity_Asset,
                userData,
                cancellationToken,
                updateEvent,
                dependencyAssetEvent
            );
        }
    }
}