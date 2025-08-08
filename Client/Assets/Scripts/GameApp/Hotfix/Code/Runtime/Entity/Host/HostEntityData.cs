using GameApp.GFEntity;
using UnityEngine;
using GameFramework;

namespace GameApp.Hotfix.GFEntity
{
    public sealed class HostEntityData : BaseEntityData
    {
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

        public static HostEntityData Create(int id, int typeId, Vector3 position, Quaternion rotation, float moveSpeed)
        {
            HostEntityData _data = ReferencePool.Acquire<HostEntityData>();
            _data.m_Id = id;
            _data.m_TypeId = typeId;
            _data.m_Position = position;
            _data.m_Rotation = rotation;
            _data.m_MoveSpeed = moveSpeed;
            return _data;
        }

        public override void Clear()
        {
            base.Clear();

            m_MoveSpeed = 0;
        }
    }
}