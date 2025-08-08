using System.Collections;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using Sirenix.OdinInspector;

namespace GameApp
{
    public sealed class CameraComponent : GameFrameworkComponent
    {
        [SerializeField]
        private Camera m_UICamera;

        [ShowInInspector]
        [ReadOnly]
        private Camera m_SceneCamera;

        /// <summary>
        /// UI相机
        /// </summary>
        public Camera UICamera => m_UICamera;

        /// <summary>
        /// 场景相机
        /// </summary>
        public Camera SceneCamera => m_SceneCamera;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            GameEntry.Event.Subscribe(SceneCameraEnableEventArgs.EventId, OnSceneCameraEnable);
            GameEntry.Event.Subscribe(SceneCameraDisableEventArgs.EventId, OnSceneCameraDisable);
        }

        private void OnSceneCameraEnable(object sender, GameEventArgs e)
        {
            SceneCameraEnableEventArgs eventArgs = e as SceneCameraEnableEventArgs;
            if (eventArgs == null)
            {
                return;
            }

            m_SceneCamera = eventArgs.SceneCamera;
        }

        private void OnSceneCameraDisable(object sender, GameEventArgs e)
        {
            SceneCameraDisableEventArgs eventArgs = e as SceneCameraDisableEventArgs;
            if (eventArgs == null)
            {
                return;
            }

            if (m_SceneCamera == eventArgs.SceneCamera)
            {
                m_SceneCamera = null;
            }
        }

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