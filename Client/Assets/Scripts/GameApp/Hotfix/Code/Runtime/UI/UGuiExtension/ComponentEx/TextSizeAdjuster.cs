using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameApp.Hotfix
{
    public sealed class TextSizeAdjuster : MonoBehaviour
    {
        [Header("尺寸配置")]
        [SerializeField, Tooltip("行数到高度的映射配置")]
        private List<LineHeightMapping> m_LineMappings = new();

        [SerializeField, Tooltip("宽度自适应")]
        private bool m_EnableWidthAutoFit = true;

        [SerializeField, Tooltip("最小宽度")]
        private float m_MinWidth = 50f;

        [SerializeField, Tooltip("最大宽度")]
        private float m_MaxWidth = 300f;

        [Header("目标设置")]
        [SerializeField, Tooltip("目标RectTransform（为空时使用当前对象）")]
        private RectTransform m_TargetRectTransform;

        [SerializeField, Tooltip("文本组件引用（为空时自动获取）")]
        private TextMeshProUGUI m_TextComponent;

        [Header("高度计算")]
        [SerializeField, Tooltip("每行基础高度")]
        private float m_BaseLineHeight = 30f;

        [SerializeField, Tooltip("顶部边距")]
        private float m_TopPadding = 0f;

        [SerializeField, Tooltip("底部边距")]
        private float m_BottomPadding = 0f;

        [SerializeField, Tooltip("宽度额外边距")]
        private float m_WidthPadding = 0f;

        [System.Serializable]
        public class LineHeightMapping
        {
            public int LineCount;
            public float Height;
            public string Description;
        }

        private void Awake()
        {
            InitializeReferences();
            SortMappings();
        }

        /// <summary>
        /// 根据传入的字符串调整目标尺寸。
        /// </summary>
        /// <param name="text">要显示的文本。</param>
        public void AdjustSizeWithText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("传入的文本为空");
                return;
            }

            if (!EnsureComponents())
            {
                Debug.LogError("组件初始化失败，无法调整尺寸");
                return;
            }

            // 设置文本
            m_TextComponent.text = text;
            // 计算并应用尺寸
            CalculateAndApplySize();
        }

        /// <summary>
        /// 根据当前文本调整目标尺寸。
        /// </summary>
        public void AdjustCurrentTextSize()
        {
            if (!EnsureComponents())
            {
                Debug.LogError("组件初始化失败，无法调整尺寸");
                return;
            }

            CalculateAndApplySize();
        }

        /// <summary>
        /// 计算并应用尺寸。
        /// </summary>
        private void CalculateAndApplySize()
        {
            float targetWidth = CalculatePreferredWidth();
            int lineCount = GetCurrentLineCount();
            float targetHeight = CalculateHeightByLineCount(lineCount);
            ApplySizeToTarget(targetWidth, targetHeight);
        }

        /// <summary>
        /// 获取当前文本的实际行数。
        /// </summary>
        private int GetCurrentLineCount()
        {
            // 强制 TMP 计算布局。
            m_TextComponent.ForceMeshUpdate(true);
            return m_TextComponent.textInfo.lineCount;
        }

        /// <summary>
        /// 计算文本的首选宽度。
        /// </summary>
        private float CalculatePreferredWidth()
        {
            if (!m_EnableWidthAutoFit)
            {
                // 如果不启用宽度自适应，返回当前宽度或最小宽度。
                return null != m_TargetRectTransform ? m_TargetRectTransform.rect.width : m_MinWidth;
            }

            // 先禁用自动换行以获取实际所需宽度。
            bool originalWordWrap = m_TextComponent.enableWordWrapping;
            m_TextComponent.enableWordWrapping = false;
            // 计算文本的实际所需宽度。
            float preferredWidth = m_TextComponent.GetPreferredValues(m_TextComponent.text).x;
            // 恢复原来的自动换行设置。
            m_TextComponent.enableWordWrapping = originalWordWrap;
            // 应用限制和边距。
            return Mathf.Clamp(preferredWidth + m_WidthPadding, m_MinWidth, m_MaxWidth);
        }

        /// <summary>
        /// 根据行数计算目标高度。
        /// </summary>
        private float CalculateHeightByLineCount(int lineCount)
        {
            if (m_LineMappings.Count > 0)
            {
                // 使用映射配置。
                foreach (LineHeightMapping mapping in m_LineMappings)
                {
                    if (lineCount <= mapping.LineCount)
                    {
                        return mapping.Height + m_TopPadding + m_BottomPadding;
                    }
                }

                // 如果超过所有映射，使用最后一个。
                return m_LineMappings[^1].Height + m_TopPadding + m_BottomPadding;
            }
            else
            {
                // 默认计算：每行高度 * 行数 + 边距。
                return (lineCount * m_BaseLineHeight) + m_TopPadding + m_BottomPadding;
            }
        }

        /// <summary>
        /// 应用尺寸到目标
        /// </summary>
        private void ApplySizeToTarget(float targetWidth, float targetHeight)
        {
            if (null == m_TargetRectTransform)
            {
                Debug.LogWarning("目标RectTransform未设置");
                return;
            }

            Vector2 currentSize = m_TargetRectTransform.rect.size;
            Vector2 targetSize = new Vector2(targetWidth, targetHeight);
            // 如果尺寸相同，直接返回。
            if (Vector2.Distance(currentSize, targetSize) < 0.1f) return;
            // 直接设置尺寸。
            m_TargetRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
            m_TargetRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);

#if UNITY_EDITOR
            Debug.Log($"尺寸调整完成: 宽度={targetWidth:F1}, 高度={targetHeight:F1}");
#endif
        }

        /// <summary>
        /// 初始化引用
        /// </summary>
        private void InitializeReferences()
        {
            // 确保 TMP 启用自动换行（如果启用了宽度自适应）。
            if (null != m_TextComponent && m_EnableWidthAutoFit)
            {
                m_TextComponent.enableWordWrapping = true;
            }
        }

        /// <summary>
        /// 确保组件已正确初始化。
        /// </summary>
        private bool EnsureComponents()
        {
            if (null == m_TextComponent)
            {
                Debug.LogError("TextMeshProUGUI 组件未找到");
                return false;
            }

            if (null == m_TargetRectTransform)
            {
                Debug.LogError("RectTransform 组件未找到");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 排序映射配置。
        /// </summary>
        private void SortMappings()
        {
            if (m_LineMappings is { Count: > 0 })
            {
                m_LineMappings.Sort((a, b) => a.LineCount.CompareTo(b.LineCount));
            }
        }

        #region 公共API

        /// <summary>
        /// 清空所有映射配置。
        /// </summary>
        public void ClearMappings()
        {
            m_LineMappings.Clear();
        }

        /// <summary>
        /// 添加行数映射。
        /// </summary>
        public void AddLineMapping(int lineCount, float height, string description = "")
        {
            LineHeightMapping mapping = new LineHeightMapping
            {
                LineCount = lineCount,
                Height = height,
                Description = description
            };

            m_LineMappings.Add(mapping);
            SortMappings();
        }

        /// <summary>
        /// 获取当前配置信息。
        /// </summary>
        public string GetConfigInfo()
        {
            string info = $"尺寸配置信息:\n";
            info += $"行数映射数量: {m_LineMappings.Count}\n";
            info += $"每行基础高度: {m_BaseLineHeight:F1}\n";
            info += $"边距: {m_TopPadding}+{m_BottomPadding}\n";
            info += $"宽度自适应: {m_EnableWidthAutoFit}\n";
            info += $"宽度范围: {m_MinWidth:F1} ~ {m_MaxWidth:F1}\n";
            info += $"宽度边距: {m_WidthPadding:F1}\n";
            return info;
        }

        /// <summary>
        /// 获取当前尺寸。
        /// </summary>
        public Vector2 GetCurrentSize()
        {
            if (null == m_TargetRectTransform) return Vector2.zero;
            return m_TargetRectTransform.rect.size;
        }

        #endregion

        #region 编辑器功能

#if UNITY_EDITOR
        [ContextMenu("测试尺寸调整")]
        private void TestSizeAdjustment()
        {
            if (null != m_TextComponent)
            {
                AdjustSizeWithText(m_TextComponent.text);
            }
        }

        [ContextMenu("添加示例映射")]
        private void AddExampleMappings()
        {
            ClearMappings();
            AddLineMapping(1, 50f, "单行");
            AddLineMapping(2, 100f, "两行");
            AddLineMapping(3, 150f, "三行");
            AddLineMapping(4, 200f, "四行");
            Debug.Log("已添加示例行映射配置");
        }

        [ContextMenu("查看配置")]
        private void ViewConfig()
        {
            Debug.Log(GetConfigInfo());
        }
#endif

        #endregion
    }
}