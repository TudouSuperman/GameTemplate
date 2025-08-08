using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using TMPro;

namespace GameApp
{
    public abstract class UGuiFormLogic : UIFormLogic
    {
        public const int DepthFactor = 10;
        public int OriginalDepth { get; private set; }
        public int Depth => m_CachedCanvas.sortingOrder;

        private static TMP_FontAsset s_MainFont = null;
        private Canvas m_CachedCanvas = null;
        private readonly List<Canvas> m_CachedCanvasContainer = new List<Canvas>();

        protected UGuiFormView m_UGuiFormView;

        public static void SetMainFont(TMP_FontAsset mainFont)
        {
            if (mainFont == null)
            {
                Log.Error("Main font is invalid.");
                return;
            }

            s_MainFont = mainFont;
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnInit(object userData)
#else
        protected internal override void OnInit(object userData)
#endif
        {
            base.OnInit(userData);

            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            m_CachedCanvas.overrideSorting = true;
            OriginalDepth = m_CachedCanvas.sortingOrder;
            gameObject.GetOrAddComponent<GraphicRaycaster>();

            RectTransform _transform = GetComponent<RectTransform>();
            _transform.anchorMin = Vector2.zero;
            _transform.anchorMax = Vector2.one;
            _transform.anchoredPosition = Vector2.zero;
            _transform.sizeDelta = Vector2.zero;

            m_UGuiFormView = GetComponent<UGuiFormView>();
            m_UGuiFormView.OnInit();

            foreach (TextMeshProUGUI _tmp in GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                _tmp.font = s_MainFont;
                if (!string.IsNullOrEmpty(_tmp.text))
                {
                    _tmp.text = GameEntry.Localization.GetString(_tmp.text);
                }
            }
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnRecycle()
#else
        protected internal override void OnRecycle()
#endif
        {
            base.OnRecycle();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnOpen(object userData)
#else
        protected internal override void OnOpen(object userData)
#endif
        {
            base.OnOpen(userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnClose(bool isShutdown, object userData)
#else
        protected internal override void OnClose(bool isShutdown, object userData)
#endif
        {
            base.OnClose(isShutdown, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnPause()
#else
        protected internal override void OnPause()
#endif
        {
            base.OnPause();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnResume()
#else
        protected internal override void OnResume()
#endif
        {
            base.OnResume();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnCover()
#else
        protected internal override void OnCover()
#endif
        {
            base.OnCover();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnReveal()
#else
        protected internal override void OnReveal()
#endif
        {
            base.OnReveal();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnRefocus(object userData)
#else
        protected internal override void OnRefocus(object userData)
#endif
        {
            base.OnRefocus(userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#else
        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#endif
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
#else
        protected internal override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
#endif
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);

            int _oldDepth = Depth;
            int _deltaDepth = UGuiGroupHelper.DepthFactor * uiGroupDepth + DepthFactor * depthInUIGroup - _oldDepth + OriginalDepth;
            GetComponentsInChildren(true, m_CachedCanvasContainer);
            foreach (Canvas _canvas in m_CachedCanvasContainer)
            {
                _canvas.sortingOrder += _deltaDepth;
            }
            m_CachedCanvasContainer.Clear();
        }
    }
}