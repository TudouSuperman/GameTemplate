using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using Sirenix.OdinInspector;

namespace GameApp
{
    public sealed class CameraComponent : GameFrameworkComponent
    {
        [SerializeField]
        private Camera m_UICamera;

        [SerializeField]
        private Camera m_DefaultSceneCamera;

        [ShowInInspector]
        [ReadOnly]
        private Camera m_CurrentSceneCamera;

        /// <summary>
        /// UI相机。
        /// </summary>
        public Camera UICamera => m_UICamera;

        /// <summary>
        /// 当前场景相机。
        /// </summary>
        public Camera CurrentSceneCamera => m_CurrentSceneCamera;

        protected override void Awake()
        {
            base.Awake();

            SetCurrentSceneCamera(m_DefaultSceneCamera);
        }

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

            SetCurrentSceneCamera(eventArgs.SceneCamera);
        }

        private void OnSceneCameraDisable(object sender, GameEventArgs e)
        {
            SceneCameraDisableEventArgs eventArgs = e as SceneCameraDisableEventArgs;
            if (eventArgs == null)
            {
                return;
            }

            if (m_CurrentSceneCamera == eventArgs.SceneCamera)
            {
                SetCurrentSceneCamera(eventArgs.SceneCamera);
            }
        }

        private void SetCurrentSceneCamera(Camera sceneCamera)
        {
            m_CurrentSceneCamera = sceneCamera;
            if (m_CurrentSceneCamera == m_DefaultSceneCamera)
            {
                m_DefaultSceneCamera.enabled = true;
            }
            else
            {
                m_DefaultSceneCamera.enabled = false;
                UniversalAdditionalCameraData currentCameraData = m_CurrentSceneCamera.GetUniversalAdditionalCameraData();
                if (currentCameraData.renderType != CameraRenderType.Base)
                {
                    throw new GameFrameworkException("Scene camera must be base camera.");
                }

                if (!currentCameraData.cameraStack.Contains(m_UICamera))
                {
                    currentCameraData.cameraStack.Add(m_UICamera);
                }
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