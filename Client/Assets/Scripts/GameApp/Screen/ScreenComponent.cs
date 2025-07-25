using UnityEngine;
using UnityEngine.UI;
using GameFramework;
using UnityGameFramework.Runtime;
using Sirenix.OdinInspector;

namespace GameApp
{
    [InfoBox("目前只针对于 UGui")]
    public sealed class ScreenComponent : GameFrameworkComponent
    {
        [SerializeField]
        private CanvasScaler m_UIRootCanvasScaler;

        [SerializeField, OnValueChanged(nameof(OnDefaultStandardSizeChange)), DisableInPlayMode]
        private int m_DefaultStandardWidth = 1920;

        [SerializeField, OnValueChanged(nameof(OnDefaultStandardSizeChange)), DisableInPlayMode]
        private int m_DefaultStandardHeight = 1080;

        /// <summary>
        /// 标准屏幕宽。
        /// </summary>
        [ShowInInspector, ReadOnly]
        public int StandardWidth { get; private set; }

        /// <summary>
        /// 标准屏幕高。
        /// </summary>
        [ShowInInspector, ReadOnly]
        public int StandardHeight { get; private set; }

        /// <summary>
        /// 屏幕宽度。
        /// </summary>
        [ShowInInspector, ReadOnly]
        public int Width { get; private set; }

        /// <summary>
        /// 屏幕高度。
        /// </summary>
        [ShowInInspector, ReadOnly]
        public int Height { get; private set; }

        /// <summary>
        /// 屏幕安全区域。
        /// </summary>
        [ShowInInspector, ReadOnly]
        public Rect SafeArea { get; private set; }

        /// <summary>
        /// UI 宽。
        /// </summary>
        [ShowInInspector, ReadOnly]
        public float UIWidth { get; private set; }

        /// <summary>
        /// UI 高。
        /// </summary>
        [ShowInInspector, ReadOnly]
        public float UIHeight { get; private set; }

        /// <summary>
        /// 标准屏幕比例（高/宽）。
        /// </summary>
        [ShowInInspector, ReadOnly]
        public float StandardVerticalRatio { get; private set; }

        /// <summary>
        /// 标准屏幕比例（宽/高）。
        /// </summary>
        [ShowInInspector, ReadOnly]
        public float StandardHorizontalRatio { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (m_UIRootCanvasScaler == null)
            {
                Log.Error("UI Root CanvasScaler is not assigned!");
                return;
            }

            Set(m_DefaultStandardWidth, m_DefaultStandardHeight);
        }

        /// <summary>
        /// 设置屏幕基准参数。
        /// </summary>
        /// <param name="standardWidth">标准宽度。</param>
        /// <param name="standardHeight">标准高度。</param>
        public void Set(int standardWidth, int standardHeight)
        {
            if (standardWidth <= 0 || standardHeight <= 0)
            {
                Log.Error(Utility.Text.Format("Invalid screen size: {0} x {1}", standardWidth, standardHeight));
                return;
            }

            if (m_UIRootCanvasScaler == null)
            {
                Log.Error("CanvasScaler reference is missing!");
                return;
            }

            SafeArea = Screen.safeArea;
            Width = Screen.width;
            Height = Screen.height;
            Log.Info(Utility.Text.Format("Screen safe area: {0}", SafeArea));
            Log.Info(Utility.Text.Format("Physical size: {0} x {1}", Width, Height));
            // 设置标准参数。
            StandardWidth = standardWidth;
            StandardHeight = standardHeight;
            Log.Info(Utility.Text.Format("Reference resolution: {0} x {1}", StandardWidth, StandardHeight));
            // 计算比例值（缓存计算结果避免重复计算）。
            StandardVerticalRatio = (float)StandardHeight / StandardWidth;
            StandardHorizontalRatio = (float)StandardWidth / StandardHeight;
            // 更新 CanvasScaler 设置。
            m_UIRootCanvasScaler.referenceResolution = new Vector2(StandardWidth, StandardHeight);
            // 计算屏幕适配模式。
            float currentRatio = SafeArea.height / SafeArea.width;
            m_UIRootCanvasScaler.matchWidthOrHeight = currentRatio > StandardVerticalRatio ? 0 : 1;
            // 强制更新Canvas布局。
            Canvas.ForceUpdateCanvases();
            // 获取实际UI尺寸。
            Vector2 sizeDelta = m_UIRootCanvasScaler.GetComponent<RectTransform>().sizeDelta;
            UIWidth = sizeDelta.x;
            UIHeight = sizeDelta.y;
            Log.Info(Utility.Text.Format("UI dimensions: {0} x {1}", UIWidth, UIHeight));
        }

        /// <summary>
        /// 编辑器回调：当默认尺寸变更时更新 CanvasScaler。
        /// </summary>
        private void OnDefaultStandardSizeChange()
        {
            if (!Application.isPlaying && m_UIRootCanvasScaler != null)
            {
                m_UIRootCanvasScaler.referenceResolution = new Vector2(m_DefaultStandardWidth, m_DefaultStandardHeight);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器调试：模拟屏幕尺寸变化。
        /// </summary>
        [Button("Simulate Screen Change")]
        private void SimulateScreenChange()
        {
            Set(m_DefaultStandardWidth, m_DefaultStandardHeight);
        }
#endif
    }
}