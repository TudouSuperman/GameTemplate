using UnityEngine;
using TMPro;

namespace GameApp.Hotfix
{
    /// <summary>
    /// 自适应文本宽度组件。
    /// 根据文本内容自动调整父节点 RectTransform 的宽度。
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public sealed class AutoFitTextWidthCom : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_TargetText = null;

        [SerializeField]
        private float m_MinWidth = 100f;

        [SerializeField]
        private float m_MaxWidth = 300f;

        [SerializeField]
        private Vector2 m_TextPadding = new Vector2(20f, 0f);

        [SerializeField]
        [Tooltip("是否启用自动刷新布局")]
        private bool m_AutoRefresh = true;

#if UNITY_EDITOR
        [Header("测试配置")]
        [SerializeField]
        [Tooltip("测试用文本数组，用于验证组件功能")]
        private string[] m_TestTexts = new string[]
        {
            "短文本",
            "中等长度的文本内容",
            "这是一个非常非常长的文本内容，用于测试最大宽度限制情况"
        };

        [SerializeField]
        [Tooltip("当前测试文本索引")]
        private int m_CurrentTestIndex = 0;
#endif

        private RectTransform m_CachedRectTransform = null;
        private RectTransform m_TextRectTransform = null;
        private bool m_IsInitialized = false;
        private string m_LastText = null;
        private float m_LastFontSize = 0;
        private TMP_FontAsset m_LastFont = null;

        /// <summary>
        /// 目标文本组件。
        /// </summary>
        public TextMeshProUGUI TargetText
        {
            get => m_TargetText;
            set
            {
                if (m_TargetText != value)
                {
                    m_TargetText = value;
                    m_TextRectTransform = null;
                    RefreshLayout();
                }
            }
        }

        /// <summary>
        /// 最小宽度。
        /// </summary>
        public float MinWidth
        {
            get => m_MinWidth;
            set
            {
                if (!Mathf.Approximately(m_MinWidth, value))
                {
                    m_MinWidth = value;
                    RefreshLayout();
                }
            }
        }

        /// <summary>
        /// 最大宽度。
        /// </summary>
        public float MaxWidth
        {
            get => m_MaxWidth;
            set
            {
                if (!Mathf.Approximately(m_MaxWidth, value))
                {
                    m_MaxWidth = value;
                    RefreshLayout();
                }
            }
        }

        /// <summary>
        /// 文本内边距。
        /// </summary>
        public Vector2 TextPadding
        {
            get => m_TextPadding;
            set
            {
                if (m_TextPadding != value)
                {
                    m_TextPadding = value;
                    RefreshLayout();
                }
            }
        }

        /// <summary>
        /// 是否启用自动刷新。
        /// </summary>
        public bool AutoRefresh
        {
            get => m_AutoRefresh;
            set => m_AutoRefresh = value;
        }

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            RefreshLayout();
        }

        private void OnEnable()
        {
            RefreshLayout();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (m_AutoRefresh) CheckAndRefresh();
                return;
            }
#endif

            if (m_AutoRefresh && m_TargetText != null)
            {
                CheckAndRefresh();
            }
        }

        private void OnValidate()
        {
            m_MinWidth = Mathf.Max(0, m_MinWidth);
            m_MaxWidth = Mathf.Max(m_MinWidth, m_MaxWidth);

            if (m_TargetText != null && m_CachedRectTransform != null)
            {
                RefreshLayout();
            }
        }

        /// <summary>
        /// 初始化组件。
        /// </summary>
        private void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }

            m_CachedRectTransform = GetComponent<RectTransform>();

            if (m_TargetText == null)
            {
                m_TargetText = GetComponentInChildren<TextMeshProUGUI>();
            }

            if (m_TargetText != null)
            {
                m_TextRectTransform = m_TargetText.rectTransform;
            }

            m_IsInitialized = true;
        }

        /// <summary>
        /// 检查是否需要刷新布局。
        /// </summary>
        private void CheckAndRefresh()
        {
            if (m_TargetText == null)
            {
                return;
            }

            bool _needRefresh = false;

            // 检查文本内容是否变化。
            if (m_TargetText.text != m_LastText)
            {
                m_LastText = m_TargetText.text;
                _needRefresh = true;
            }

            // 检查字体大小是否变化。
            if (!Mathf.Approximately(m_TargetText.fontSize, m_LastFontSize))
            {
                m_LastFontSize = m_TargetText.fontSize;
                _needRefresh = true;
            }

            // 检查字体是否变化。
            if (m_TargetText.font != m_LastFont)
            {
                m_LastFont = m_TargetText.font;
                _needRefresh = true;
            }

            if (_needRefresh)
            {
                RefreshLayout();
            }
        }

        /// <summary>
        /// 刷新布局。
        /// </summary>
        public void RefreshLayout()
        {
            if (!m_IsInitialized)
            {
                Initialize();
            }

            if (m_TargetText == null || m_CachedRectTransform == null)
            {
                Debug.LogWarning("AutoFitTextWidthComponent: TargetText or RectTransform is null.");
                return;
            }

            // 确保文本 RectTransform 引用有效。
            if (m_TextRectTransform == null)
            {
                m_TextRectTransform = m_TargetText.rectTransform;
            }

            // 强制计算文本布局（对于 TMP 需要这一步来更新 preferredWidth）。
            m_TargetText.ForceMeshUpdate();
            // 获取文本的实际宽度。
            float _preferredWidth = m_TargetText.preferredWidth;
            // 如果获取到无效的宽度，尝试使用备用方法。
            if (Mathf.Approximately(_preferredWidth, 0f) && !string.IsNullOrEmpty(m_TargetText.text))
            {
                // 尝试使用文本边界。
                _preferredWidth = m_TargetText.GetRenderedValues().x;
                // 如果还是无效，使用字体大小估算。
                if (Mathf.Approximately(_preferredWidth, 0f))
                {
                    _preferredWidth = m_TargetText.text.Length * m_TargetText.fontSize * 0.5f;
                }
            }

            // 加上内边距。
            float _totalWidth = _preferredWidth + m_TextPadding.x * 2;
            // 限制宽度在最小和最大值之间。
            float _clampedWidth = Mathf.Clamp(_totalWidth, m_MinWidth, m_MaxWidth);
            // 设置父节点宽度。
            Vector2 _sizeDelta = m_CachedRectTransform.sizeDelta;
            _sizeDelta.x = _clampedWidth;
            m_CachedRectTransform.sizeDelta = _sizeDelta;
        }

        /// <summary>
        /// 设置目标文本。
        /// </summary>
        /// <param name="text">文本组件。</param>
        public void SetTargetText(TextMeshProUGUI text)
        {
            m_TargetText = text;
            m_TextRectTransform = text != null ? text.rectTransform : null;
            RefreshLayout();
        }

        /// <summary>
        /// 设置宽度范围。
        /// </summary>
        /// <param name="minWidth">最小宽度。</param>
        /// <param name="maxWidth">最大宽度。</param>
        public void SetWidthRange(float minWidth, float maxWidth)
        {
            m_MinWidth = Mathf.Max(0, minWidth);
            m_MaxWidth = Mathf.Max(m_MinWidth, maxWidth);
            RefreshLayout();
        }

        /// <summary>
        /// 获取当前文本的实际宽度（不包括内边距）。
        /// </summary>
        public float GetTextPreferredWidth()
        {
            if (m_TargetText == null)
            {
                return 0f;
            }

            m_TargetText.ForceMeshUpdate();
            return m_TargetText.preferredWidth;
        }

        /// <summary>
        /// 获取当前文本的总宽度（包括内边距）。
        /// </summary>
        public float GetTotalPreferredWidth()
        {
            return GetTextPreferredWidth() + m_TextPadding.x * 2;
        }

        /// <summary>
        /// 获取当前父节点宽度。
        /// </summary>
        public float GetCurrentWidth()
        {
            if (m_CachedRectTransform == null)
            {
                return 0f;
            }

            return m_CachedRectTransform.sizeDelta.x;
        }

        /// <summary>
        /// 检查是否需要更新布局。
        /// </summary>
        public bool NeedsLayoutUpdate()
        {
            if (m_TargetText == null || m_CachedRectTransform == null)
            {
                return false;
            }

            float _totalWidth = GetTotalPreferredWidth();
            float _currentWidth = m_CachedRectTransform.sizeDelta.x;
            float _clampedWidth = Mathf.Clamp(_totalWidth, m_MinWidth, m_MaxWidth);

            return !Mathf.Approximately(_currentWidth, _clampedWidth);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 强制立即刷新布局。
        /// </summary>
        [ContextMenu("Force Refresh Layout")]
        public void ForceRefreshLayout()
        {
            m_LastText = null;
            m_LastFontSize = 0;
            m_LastFont = null;
            RefreshLayout();
        }

        /// <summary>
        /// 测试功能 - 应用下一个测试文本。
        /// </summary>
        [ContextMenu("测试下一个文本")]
        public void ApplyNextTestText()
        {
            if (m_TestTexts == null || m_TestTexts.Length == 0)
            {
                Debug.LogWarning("没有配置测试文本");
                return;
            }

            if (m_TargetText == null)
            {
                Debug.LogWarning("目标文本组件为空，无法进行测试");
                return;
            }

            m_CurrentTestIndex = (m_CurrentTestIndex + 1) % m_TestTexts.Length;
            string _testText = m_TestTexts[m_CurrentTestIndex];
            m_TargetText.text = _testText;

            RefreshLayout();

            Debug.Log($"应用测试文本 {m_CurrentTestIndex + 1}/{m_TestTexts.Length}: \"{_testText}\"");
            Debug.Log($"文本宽度: {GetTextPreferredWidth():F1}px, 总宽度: {GetTotalPreferredWidth():F1}px, 父节点宽度: {m_CachedRectTransform.sizeDelta.x:F1}px");
        }

        /// <summary>
        /// 测试功能 - 应用指定索引的测试文本。
        /// </summary>
        /// <param name="index">测试文本索引。</param>
        public void ApplyTestText(int index)
        {
            if (m_TestTexts == null || m_TestTexts.Length == 0)
            {
                Debug.LogWarning("没有配置测试文本");
                return;
            }

            if (index < 0 || index >= m_TestTexts.Length)
            {
                Debug.LogWarning($"索引 {index} 超出范围 (0-{m_TestTexts.Length - 1})");
                return;
            }

            if (m_TargetText == null)
            {
                Debug.LogWarning("目标文本组件为空，无法进行测试");
                return;
            }

            m_CurrentTestIndex = index;
            string _testText = m_TestTexts[m_CurrentTestIndex];
            m_TargetText.text = _testText;

            RefreshLayout();

            Debug.Log($"应用测试文本 {m_CurrentTestIndex + 1}/{m_TestTexts.Length}: \"{_testText}\"");
            Debug.Log($"文本宽度: {GetTextPreferredWidth():F1}px, 总宽度: {GetTotalPreferredWidth():F1}px, 父节点宽度: {m_CachedRectTransform.sizeDelta.x:F1}px");
        }

        /// <summary>
        /// 测试功能 - 测试所有配置的文本。
        /// </summary>
        [ContextMenu("测试所有文本")]
        public void TestAllTexts()
        {
            if (m_TestTexts == null || m_TestTexts.Length == 0)
            {
                Debug.LogWarning("没有配置测试文本");
                return;
            }

            if (m_TargetText == null)
            {
                Debug.LogWarning("目标文本组件为空，无法进行测试");
                return;
            }

            Debug.Log($"开始测试 {m_TestTexts.Length} 个文本...");

            string _originalText = m_TargetText.text;

            for (int _i = 0; _i < m_TestTexts.Length; _i++)
            {
                m_CurrentTestIndex = _i;
                m_TargetText.text = m_TestTexts[_i];
                RefreshLayout();

                Debug.Log($"[{_i + 1}/{m_TestTexts.Length}] \"{m_TestTexts[_i]}\" | " +
                          $"文本宽: {GetTextPreferredWidth():F1}px, 总宽: {GetTotalPreferredWidth():F1}px, " +
                          $"父节点宽: {m_CachedRectTransform.sizeDelta.x:F1}px");
            }

            // 恢复原始文本。
            m_TargetText.text = _originalText;
            RefreshLayout();

            Debug.Log("测试完成，已恢复原始文本");
        }

        /// <summary>
        /// 测试功能 - 随机生成长文本进行测试。
        /// </summary>
        [ContextMenu("测试随机长文本")]
        public void TestRandomLongText()
        {
            if (m_TargetText == null)
            {
                Debug.LogWarning("目标文本组件为空，无法进行测试");
                return;
            }

            // 生成随机长度的文本。
            int _textLength = Random.Range(10, 100);
            string _randomText = "测试长文本";
            for (int _i = 0; _i < _textLength; _i++)
            {
                _randomText += "测试";
            }

            m_TargetText.text = _randomText;
            RefreshLayout();

            Debug.Log($"应用随机长文本 (长度: {_textLength * 2 + 5} 字符): \"{_randomText.Substring(0, Mathf.Min(30, _randomText.Length))}...\"");
            Debug.Log($"文本宽度: {GetTextPreferredWidth():F1}px, 总宽度: {GetTotalPreferredWidth():F1}px, 父节点宽度: {m_CachedRectTransform.sizeDelta.x:F1}px");
        }

        /// <summary>
        /// 测试功能 - 验证宽度限制是否正常工作。
        /// </summary>
        [ContextMenu("验证宽度限制")]
        public void ValidateWidthLimits()
        {
            if (m_TargetText == null || m_CachedRectTransform == null)
            {
                Debug.LogWarning("组件未正确初始化，无法验证");
                return;
            }

            float _currentWidth = m_CachedRectTransform.sizeDelta.x;
            float _textWidth = GetTextPreferredWidth();
            float _totalWidth = GetTotalPreferredWidth();

            bool _isWithinLimits = _currentWidth >= m_MinWidth && _currentWidth <= m_MaxWidth;

            Debug.Log($"宽度限制验证:");
            Debug.Log($"- 父节点宽度: {_currentWidth:F1}px (范围: {m_MinWidth}-{m_MaxWidth}px)");
            Debug.Log($"- 文本宽度: {_textWidth:F1}px");
            Debug.Log($"- 总需求宽度: {_totalWidth:F1}px");
            Debug.Log($"- 是否在限制范围内: {(_isWithinLimits ? "✓" : "✗")}");

            if (!_isWithinLimits)
            {
                Debug.LogError($"父节点宽度超出限制范围!");
            }
        }
#endif
    }
}