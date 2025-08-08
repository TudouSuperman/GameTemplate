using UnityGameFramework.Runtime;

namespace GameApp.Hotfix
{
    public static class EntityExtension
    {
        public static bool TryGetEntity(this EntityComponent entityComponent, int serialId, out BaseEntityLogic entityLogic)
        {
            entityLogic = null;
            Entity entity = entityComponent.GetEntity(serialId);
            if (entity == null)
            {
                return false;
            }

            entityLogic = (BaseEntityLogic)entity.Logic;
            return true;
        }

        public static void TryHideEntity(this EntityComponent entityComponent, int serialId)
        {
            if (entityComponent.IsLoadingEntity(serialId) || entityComponent.HasEntity(serialId))
            {
                entityComponent.HideEntity(serialId);
            }
        }
    }
}