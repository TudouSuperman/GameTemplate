using UnityEngine;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameApp.Hotfix
{
    /// <summary>
    /// 控制浮动窗口的位置行为，使其跟随鼠标或指定位置，
    /// 并确保窗口保持在屏幕边界内（可配置四向安全边距）。
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class WindowFloatingCom : MonoBehaviour
    {
        [Header("UI 屏幕 Canvas")]
        [Tooltip("UI 屏幕 Canvas 组件（用于坐标计算）")]
        [SerializeField]
        private Canvas m_UICanvas;

        [Header("位置偏移")]
        [Tooltip("相对于鼠标位置的偏移量")]
        [SerializeField]
        private Vector2 m_MouseOffset = Vector2.zero;

        [Header("安全边距设置")]
        [Tooltip("是否启用安全边距")]
        [SerializeField]
        private bool m_UseSafeAreaPadding = true;

        [Tooltip("左侧安全边距（像素）")]
        [SerializeField]
        private float m_PaddingLeft = 10f;

        [Tooltip("右侧安全边距（像素）")]
        [SerializeField]
        private float m_PaddingRight = 10f;

        [Tooltip("顶部安全边距（像素）")]
        [SerializeField]
        private float m_PaddingTop = 10f;

        [Tooltip("底部安全边距（像素）")]
        [SerializeField]
        private float m_PaddingBottom = 10f;

        private RectTransform m_SelfWindowRect;
        private Vector2 m_UICanvasSizeCache;
        private Vector2 m_SelfWindowSizeCache;

        private void Awake()
        {
            if (null == m_UICanvas)
            {
                transform.GetComponentInParent<Canvas>();
            }

            m_UICanvasSizeCache = ((RectTransform)m_UICanvas.transform).rect.size;
            m_SelfWindowRect = (RectTransform)transform;
            m_SelfWindowSizeCache = m_SelfWindowRect.rect.size;
            m_SelfWindowRect.anchorMin = new Vector2(0, 1f);
            m_SelfWindowRect.anchorMax = new Vector2(0, 1f);
            m_SelfWindowRect.pivot = new Vector2(0, 1f);
        }

        /// <summary>
        /// 刷新尺寸缓存（当窗口或 Canvas 尺寸变化时调用）。
        /// </summary>
        public void RefreshSizeCache()
        {
            m_UICanvasSizeCache = ((RectTransform)m_UICanvas.transform).rect.size;
            m_SelfWindowSizeCache = m_SelfWindowRect.rect.size;
        }

        /// <summary>
        /// 使窗口跟随鼠标位置。
        /// </summary>
        public void FollowCursor() => SetPosition((Vector2)transform.parent.InverseTransformPoint(GameEntry.Cursor.ScreenPosition) + m_MouseOffset);

        /// <summary>
        /// 将窗口移动到指定的世界坐标位置。
        /// </summary>
        /// <param name="worldPos">目标世界坐标位置。</param>
        public void MoveToWorldPosition(Vector3 worldPos) => SetPosition(transform.parent.InverseTransformPoint(worldPos));

        /// <summary>
        /// 设置窗口的目标位置（自动应用边界约束）。
        /// </summary>
        /// <param name="targetPosition">父级坐标系中的目标位置。</param>
        private void SetPosition(Vector2 targetPosition)
        {
            m_SelfWindowRect.localPosition = targetPosition;
            ClampToScreenBounds();

            // 将窗口位置限制在屏幕边界内（考虑安全边距）。
            void ClampToScreenBounds()
            {
                Vector2 _windowPos = m_SelfWindowRect.anchoredPosition;
                float _minX = m_UseSafeAreaPadding ? m_PaddingLeft : 0f;
                float _maxX = m_UICanvasSizeCache.x - m_SelfWindowSizeCache.x - (m_UseSafeAreaPadding ? m_PaddingRight : 0f);
                float _minY = m_SelfWindowSizeCache.y - m_UICanvasSizeCache.y + (m_UseSafeAreaPadding ? m_PaddingBottom : 0f);
                float _maxY = m_UseSafeAreaPadding ? -m_PaddingTop : 0f;
                _windowPos.x = Mathf.Clamp(_windowPos.x, _minX, _maxX);
                _windowPos.y = Mathf.Clamp(_windowPos.y, _minY, _maxY);
                m_SelfWindowRect.anchoredPosition = _windowPos;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器工具：重置安全边距为默认值。
        /// </summary>
        [ContextMenu("重置安全边距")]
        private void ResetSafeAreaPadding()
        {
            m_PaddingLeft = 10f;
            m_PaddingRight = 10f;
            m_PaddingTop = 10f;
            m_PaddingBottom = 10f;
            Log.Debug("安全边距已重置为默认值");
        }

        /// <summary>
        /// 编辑器工具：刷新尺寸缓存（用于编辑模式下预览）。
        /// </summary>
        [ContextMenu("刷新尺寸缓存")]
        private void RefreshSizeCacheEditor()
        {
            RefreshSizeCache();
            Log.Debug(Utility.Text.Format("尺寸缓存已刷新 | Canvas: {0} | Window: {1}", m_UICanvasSizeCache, m_SelfWindowSizeCache));
        }
#endif
    }
}