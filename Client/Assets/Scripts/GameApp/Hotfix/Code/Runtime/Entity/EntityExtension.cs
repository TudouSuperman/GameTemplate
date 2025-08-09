using UnityGameFramework.Runtime;

namespace GameApp.Hotfix
{
    public static class EntityExtension
    {
        public static void ShowHostEntity(this EntityComponent entityComponent, HostEntityData entityData)
        {
            entityComponent.ShowEntity(typeof(HostEntityLogic), entityData);
        }
    }
}