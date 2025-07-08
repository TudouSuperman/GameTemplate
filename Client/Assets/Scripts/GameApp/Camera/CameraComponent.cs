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

        /// <summary>
        /// 是否在摄像机视野中。
        /// </summary>
        public bool IsVisibleInCamera(Camera targetCamera, Vector3 position)
        {
            Vector3 _viewPosition = targetCamera.WorldToViewportPoint(position);
            if (_viewPosition.z < -1 || _viewPosition.z > targetCamera.farClipPlane) return false;
            return !(_viewPosition.x < 0) && !(_viewPosition.y < 0) && !(_viewPosition.x > 1) && !(_viewPosition.y > 1);
        }
    }
}