using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameApp
{
    /// <summary>
    /// UGUI 界面组辅助器。
    /// </summary>
    public class UGuiGroupHelper : UIGroupHelperBase
    {
        public const int DepthFactor = 1000;

        private int m_Depth = 0;
        private Canvas m_CachedCanvas = null;

        /// <summary>
        /// 设置界面组深度。
        /// </summary>
        /// <param name="depth">界面组深度。</param>
        public override void SetDepth(int depth)
        {
            m_Depth = depth;
            m_CachedCanvas.overrideSorting = true;
            m_CachedCanvas.sortingOrder = DepthFactor * depth;
        }

        private void Awake()
        {
            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }

        private void Start()
        {
            m_CachedCanvas.overrideSorting = true;
            m_CachedCanvas.sortingOrder = DepthFactor * m_Depth;

            RectTransform _transform = GetComponent<RectTransform>();
            _transform.anchorMin = Vector2.zero;
            _transform.anchorMax = Vector2.one;
            _transform.anchoredPosition = Vector2.zero;
            _transform.sizeDelta = Vector2.zero;
        }
    }
}
