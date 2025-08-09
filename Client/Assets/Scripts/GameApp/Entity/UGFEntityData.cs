using System;
using UnityEngine;
using GameFramework;

namespace GameApp
{
    [Serializable]
    public abstract class UGFEntityData : IReference
    {
        [SerializeField]
        protected int m_Id = 0;

        [SerializeField]
        protected int m_TypeId = 0;

        [SerializeField]
        protected Vector3 m_Position = Vector3.zero;

        [SerializeField]
        protected Quaternion m_Rotation = Quaternion.identity;

        /// <summary>
        /// 实体编号。
        /// </summary>
        public int Id => m_Id;

        /// <summary>
        /// 实体类型编号。
        /// </summary>
        public int TypeId => m_TypeId;

        /// <summary>
        /// 实体位置。
        /// </summary>
        public Vector3 Position
        {
            get => m_Position;
            set => m_Position = value;
        }

        /// <summary>
        /// 实体朝向。
        /// </summary>
        public Quaternion Rotation
        {
            get => m_Rotation;
            set => m_Rotation = value;
        }

        public virtual void Clear()
        {
            m_Id = 0;
            m_TypeId = 0;
            m_Position = Vector3.zero;
            Rotation = Quaternion.identity;
        }
    }
}