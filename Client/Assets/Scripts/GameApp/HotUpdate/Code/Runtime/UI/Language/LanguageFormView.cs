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
        public event GameFrameworkAction OnOkClicked;

        public override void OnInit()
        {
            m_ChineseToggle.onValueChanged.AddListener((isOn) => { OnChineseChanged?.Invoke(isOn); });
            m_EnglishToggle.onValueChanged.AddListener((isOn) => { OnEnglishChanged?.Invoke(isOn); });
            m_OKButton.onClick.AddListener(() => { OnOkClicked?.Invoke(); });
        }

        public void SetChinese(bool flag) => m_ChineseToggle.isOn = flag;
        public void SetEnglish(bool flag) => m_EnglishToggle.isOn = flag;
    }
}