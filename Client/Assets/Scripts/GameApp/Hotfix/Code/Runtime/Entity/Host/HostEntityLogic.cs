using GameFramework;

namespace GameApp.Hotfix
{
    public sealed class HostEntityLogic : UGFEntityLogicEx
    {
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            HostEntityData _data = GetData<HostEntityData>();
            UnityGameFramework.Runtime.Log.Debug(Utility.Text.Format("{0} {1} {2} {3}", _data.SerialId, _data.TypeId, _data.Position, _data.Rotation));
        }
    }
}