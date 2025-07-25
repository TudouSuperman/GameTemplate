using GameFramework;
using CodeBind;
using GameApp.UI;

namespace GameApp.Hot.UI
{
    [MonoCodeBind('-')]
    public sealed partial class LanguageFormView : UGuiFormView
    {
        public event GameFrameworkAction<bool> OnChineseChanged;
        public event GameFrameworkAction<bool> OnEnglishChanged;
        public event GameFrameworkAction<bool> OnSpanishChanged;
        public event GameFrameworkAction<bool> OnPortugueseChanged;
        public event GameFrameworkAction OnOkClicked;

        public override void OnInit()
        {
            m_ChineseToggle.onValueChanged.AddListener((isOn) => { OnChineseChanged?.Invoke(isOn); });
            m_EnglishToggle.onValueChanged.AddListener((isOn) => { OnEnglishChanged?.Invoke(isOn); });
            m_SpanishToggle.onValueChanged.AddListener((isOn) => { OnSpanishChanged?.Invoke(isOn); });
            m_PortugueseToggle.onValueChanged.AddListener((isOn) => { OnPortugueseChanged?.Invoke(isOn); });
            m_OKButton.onClick.AddListener(() => { OnOkClicked?.Invoke(); });
        }

        public void SetChinese(bool flag) => m_ChineseToggle.isOn = flag;
        public void SetEnglish(bool flag) => m_EnglishToggle.isOn = flag;
        public void SetSpanish(bool flag) => m_SpanishToggle.isOn = flag;
        public void SetPortuguese(bool flag) => m_PortugueseToggle.isOn = flag;
    }
}