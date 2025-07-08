using UnityEngine;
using UnityEngine.UI;

namespace GameApp.UI.Extension
{
    /// <summary>
    /// 在 UGUI 中绘制雷达图组件。
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class GraphicRadar : MaskableGraphic
    {
        [Range(0f, 300f)]
        [SerializeField]
        private float m_Radius = 100f;

        [Tooltip("每个方向的百分比（0-1）")]
        [SerializeField]
        private float[] m_Percents;

        /// <summary>
        /// 动态设置单个方向的百分比。
        /// </summary>
        public void SetPercent(int index, float value)
        {
            if (index < 0 || index >= m_Percents.Length)
            {
                Debug.LogError("Index out of range");
                return;
            }

            m_Percents[index] = Mathf.Clamp01(value);
            RefreshView();
        }

        /// <summary>
        /// 动态设置所有百分比值。
        /// </summary>
        public void SetAllPercents(float[] percents)
        {
            if (percents == null || percents.Length != m_Percents.Length)
            {
                Debug.LogError("Invalid percents array length");
                return;
            }

            m_Percents = percents;
            RefreshView();
        }

        /// <summary>
        /// 刷新变动。
        /// </summary>
        public void RefreshView()
        {
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (null == m_Percents || m_Percents.Length < 3) return;
            int _count = m_Percents.Length;
            // 添加中心点。
            UIVertex _center = UIVertex.simpleVert;
            _center.color = color;
            _center.position = Vector3.zero;
            vh.AddVert(_center);
            // 添加外围点。
            for (int i = 0; i < _count; i++)
            {
                // 顺时针。
                float _angle = Mathf.PI / 2 - i * Mathf.PI * 2 / _count;
                float _percent = Mathf.Clamp01(m_Percents[i]);
                float _x = Mathf.Cos(_angle) * m_Radius * _percent;
                float _y = Mathf.Sin(_angle) * m_Radius * _percent;
                UIVertex _vert = UIVertex.simpleVert;
                _vert.color = color;
                _vert.position = new Vector3(_x, _y, 0);
                vh.AddVert(_vert);
            }

            // 三角形绘制。
            for (int i = 1; i <= _count; i++)
            {
                int _next = i == _count ? 1 : i + 1;
                vh.AddTriangle(0, i, _next);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            RefreshView();
        }
#endif
    }
}