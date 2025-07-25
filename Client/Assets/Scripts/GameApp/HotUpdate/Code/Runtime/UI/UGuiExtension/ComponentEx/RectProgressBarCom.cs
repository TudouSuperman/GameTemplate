using UnityEngine;
using GameFramework;

namespace GameApp.Hot.UI.Extension
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class RectProgressBarCom : MonoBehaviour
    {
        [SerializeField]
        [Header("宽度范围")]
        private float m_MinWidth = 100f;

        [SerializeField]
        [Header("宽度范围")]
        private float m_MaxWidth = 500f;

        [SerializeField]
        [Header("锚点模式")]
        private EAnchorMode m_AnchorMode = EAnchorMode.Left;

        private RectTransform m_TargetRect;
        private Vector2 m_OriginalPosition;
        private Vector2 m_OriginalAnchorMin;
        private Vector2 m_OriginalAnchorMax;
        private Vector2 m_OriginalPivot;

        private void Awake()
        {
            m_TargetRect = GetComponent<RectTransform>();
            SaveOriginalTransform();
            ApplyAnchorMode();
        }

        /// <summary>
        /// 保存原始变换设置。
        /// </summary>
        private void SaveOriginalTransform()
        {
            m_OriginalPosition = m_TargetRect.anchoredPosition;
            m_OriginalAnchorMin = m_TargetRect.anchorMin;
            m_OriginalAnchorMax = m_TargetRect.anchorMax;
            m_OriginalPivot = m_TargetRect.pivot;
        }

        /// <summary>
        /// 应用当前锚点模式。
        /// </summary>
        private void ApplyAnchorMode()
        {
            switch (m_AnchorMode)
            {
                case EAnchorMode.Left:
                    SetLeftAnchor();
                    break;
                case EAnchorMode.Right:
                    SetRightAnchor();
                    break;
                case EAnchorMode.Center:
                    SetCenterAnchor();
                    break;
            }
        }

        /// <summary>
        /// 设置左侧锚点（固定左侧，向右扩展）。
        /// </summary>
        private void SetLeftAnchor()
        {
            m_TargetRect.anchorMin = new Vector2(0, 0.5f);
            m_TargetRect.anchorMax = new Vector2(0, 0.5f);
            m_TargetRect.pivot = new Vector2(0, 0.5f);
            m_TargetRect.anchoredPosition = m_OriginalPosition;
        }

        /// <summary>
        /// 设置右侧锚点（固定右侧，向左扩展）。
        /// </summary>
        private void SetRightAnchor()
        {
            m_TargetRect.anchorMin = new Vector2(1, 0.5f);
            m_TargetRect.anchorMax = new Vector2(1, 0.5f);
            m_TargetRect.pivot = new Vector2(1, 0.5f);
            m_TargetRect.anchoredPosition = m_OriginalPosition;
        }

        /// <summary>
        /// 设置中心锚点（中心固定，双向扩展）。
        /// </summary>
        private void SetCenterAnchor()
        {
            m_TargetRect.anchorMin = new Vector2(0.5f, 0.5f);
            m_TargetRect.anchorMax = new Vector2(0.5f, 0.5f);
            m_TargetRect.pivot = new Vector2(0.5f, 0.5f);
            m_TargetRect.anchoredPosition = m_OriginalPosition;
        }

        /// <summary>
        /// 根据进度设置宽度。
        /// </summary>
        public void SetWidthProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);
            float _width = Mathf.Lerp(m_MinWidth, m_MaxWidth, progress);
            SetRectWidth(_width);
        }

        /// <summary>
        /// 设置矩形宽度（兼容不同 Unity 版本）。
        /// </summary>
        private void SetRectWidth(float width)
        {
            // 保存当前位置。
            Vector2 _anchoredPosition = m_TargetRect.anchoredPosition;
            // 设置宽度。
#if UNITY_2019_1_OR_NEWER
            m_TargetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
#else
            Vector2 _size = m_TargetRect.sizeDelta;
            _size.x = width;
            m_TargetRect.sizeDelta = _size;
#endif
            // 恢复位置（某些模式需要）。
            m_TargetRect.anchoredPosition = _anchoredPosition;
        }

        /// <summary>
        /// 动态改变锚点模式。
        /// </summary>
        public void ChangeAnchorMode(EAnchorMode newMode)
        {
            if (m_AnchorMode == newMode) return;
            m_AnchorMode = newMode;
            ApplyAnchorMode();
            // 重新应用当前宽度。
            SetWidthProgress(GetCurrentProgress());
        }

        /// <summary>
        /// 获取当前进度（0-1）。
        /// </summary>
        public float GetCurrentProgress()
        {
            float _currentWidth = m_TargetRect.rect.width;
            return Mathf.InverseLerp(m_MinWidth, m_MaxWidth, _currentWidth);
        }

        /// <summary>
        /// 重置为原始变换设置。
        /// </summary>
        public void ResetToOriginal()
        {
            m_TargetRect.anchorMin = m_OriginalAnchorMin;
            m_TargetRect.anchorMax = m_OriginalAnchorMax;
            m_TargetRect.pivot = m_OriginalPivot;
            m_TargetRect.anchoredPosition = m_OriginalPosition;
        }

        // 编辑器工具。
#if UNITY_EDITOR
        [ContextMenu("应用左侧锚点")]
        private void ApplyLeftAnchorEditor() => ApplyAnchorModeEditor(EAnchorMode.Left);

        [ContextMenu("应用右侧锚点")]
        private void ApplyRightAnchorEditor() => ApplyAnchorModeEditor(EAnchorMode.Right);

        [ContextMenu("应用中心锚点")]
        private void ApplyCenterAnchorEditor() => ApplyAnchorModeEditor(EAnchorMode.Center);

        [ContextMenu("重置原始锚点")]
        private void ResetAnchorEditor()
        {
            if (m_TargetRect == null)
                m_TargetRect = GetComponent<RectTransform>();
            ResetToOriginal();
            Debug.Log("已重置为原始锚点设置");
        }

        private void ApplyAnchorModeEditor(EAnchorMode mode)
        {
            if (m_TargetRect == null)
                m_TargetRect = GetComponent<RectTransform>();
            SaveOriginalTransform();
            m_AnchorMode = mode;
            ApplyAnchorMode();
            Debug.Log(Utility.Text.Format("已应用 {0} 锚点模式", mode));
        }

        private void OnValidate()
        {
            if (!Application.isPlaying && m_TargetRect == null)
            {
                m_TargetRect = GetComponent<RectTransform>();
            }

            // 在编辑器模式下预览效果。
            if (!Application.isPlaying && m_TargetRect != null)
            {
                ApplyAnchorMode();
                SetWidthProgress(GetCurrentProgress());
            }
        }
#endif
    }
}