using GameFramework;

namespace GameApp.Hotfix
{
    public sealed class HostEntityLogic : UGFEntityLogicEx
    {
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            HostEntityData _data = GetData<HostEntityData>();
            UnityEngine.Debug.Log(Utility.Text.Format("{0} {1} {2} {3} {4} {5}", _data.SerialId, _data.TypeId, _data.Damage, _data.MoveSpeed, _data.Position, _data.Rotation));
        }
    }
}