using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameApp
{
    public sealed class CameraComponent : GameFrameworkComponent
    {
        [SerializeField]
        private Camera m_MainCamera;

        [SerializeField]
        private Camera m_UICamera;

        /// <summary>
        /// 主相机。
        /// </summary>
        public Camera MainCamera => m_MainCamera;

        /// <summary>
        /// UI 相机。
        /// </summary>
        public Camera UICamera => m_UICamera;
    }
}