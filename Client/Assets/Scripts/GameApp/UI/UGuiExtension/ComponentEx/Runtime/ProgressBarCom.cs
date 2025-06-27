using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameApp.UI.Extension
{
    public sealed class ProgressBarCom : MonoBehaviour
    {
        [SerializeField]
        private Image m_ProgressBarView;

        [SerializeField]
        private TextMeshProUGUI m_ProgressTextView;

        public void RefreshView10(int currentValue, int totalValue)
        {
            m_ProgressBarView.fillAmount = Mathf.Clamp01((float)currentValue / totalValue);
            m_ProgressTextView.text = $"{currentValue}/{totalValue}";
        }

        public void RefreshView10(float currentValue, float totalValue)
        {
            m_ProgressBarView.fillAmount = Mathf.Clamp01(currentValue / totalValue);
            m_ProgressTextView.text = $"{(int)currentValue}/{(int)totalValue}";
        }
        
        public void RefreshView10Single(float currentValue, float totalValue)
        {
            m_ProgressBarView.fillAmount = Mathf.Clamp01(currentValue / totalValue);
            m_ProgressTextView.text = $"{(int)currentValue}";
        }

        public void RefreshView01(int currentValue, int totalValue)
        {
            m_ProgressBarView.fillAmount = 1f - Mathf.Clamp01((float)currentValue / totalValue);
            m_ProgressTextView.text = $"{currentValue}/{totalValue}";
        }

        public void RefreshView01(float currentValue, float totalValue)
        {
            m_ProgressBarView.fillAmount = 1f - Mathf.Clamp01(currentValue / totalValue);
            m_ProgressTextView.text = $"{(int)currentValue}/{(int)totalValue}";
        }

        public void RefreshView01Single(float currentValue, float totalValue)
        {
            m_ProgressBarView.fillAmount = 1f - Mathf.Clamp01(currentValue / totalValue);
            m_ProgressTextView.text = $"{(int)currentValue}";
        }
    }
}