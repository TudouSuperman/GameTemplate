using GameFramework;

namespace GameApp.Hotfix
{
    public sealed class HostEntityData : UGFEntityData
    {
        public static HostEntityData Create(int serialId, int typeId)
        {
            HostEntityData _data = ReferencePool.Acquire<HostEntityData>();
            _data.m_SerialId = serialId;
            _data.m_TypeId = typeId;
            return _data;
        }
    }
}