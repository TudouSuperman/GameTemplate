using UnityEngine;
using GameFramework;

namespace GameApp.Hotfix
{
    public sealed class HostEntityData : UGFEntityData
    {
        [SerializeField]
        private float m_Damage = 0;

        /// <summary>
        /// 实体伤害。
        /// </summary>
        public float Damage
        {
            get => m_Damage;
            set => m_Damage = value;
        }

        [SerializeField]
        private float m_MoveSpeed = 0;

        /// <summary>
        /// 实体移动速度。
        /// </summary>
        public float MoveSpeed
        {
            get => m_MoveSpeed;
            set => m_MoveSpeed = value;
        }

        public static HostEntityData Create(int serialId, int typeId)
        {
            HostEntityData _data = ReferencePool.Acquire<HostEntityData>();
            _data.m_SerialId = serialId;
            _data.m_TypeId = typeId;
            DRHero _drHero = GameEntry.DataTable.GetDataRow<DRHero>((int)EHeroID.Host);
            if (null == _drHero) return _data;
            _data.m_Position = _drHero.Position;
            _data.m_Rotation = _drHero.Rotation;
            DRHeroBaseData _drHeroBaseData = GameEntry.DataTable.GetDataRow<DRHeroBaseData>(_drHero.BaseDataId);
            if (null == _drHeroBaseData) return _data;
            _data.m_Damage = _drHeroBaseData.BaseDamage;
            _data.m_MoveSpeed = _drHeroBaseData.BaseMoveSpeed;
            return _data;
        }

        public override void Clear()
        {
            base.Clear();

            m_Damage = 0;
            m_MoveSpeed = 0;
        }
    }
}