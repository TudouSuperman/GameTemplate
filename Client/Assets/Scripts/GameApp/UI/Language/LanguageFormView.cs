using CodeBind;
using GameFramework;

namespace GameApp.UI
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
            ChineseToggle.onValueChanged.AddListener((isOn) => { OnChineseChanged?.Invoke(isOn); });
            EnglishToggle.onValueChanged.AddListener((isOn) => { OnEnglishChanged?.Invoke(isOn); });
            SpanishToggle.onValueChanged.AddListener((isOn) => { OnSpanishChanged?.Invoke(isOn); });
            PortugueseToggle.onValueChanged.AddListener((isOn) => { OnPortugueseChanged?.Invoke(isOn); });
            OKButton.onClick.AddListener(() => { OnOkClicked?.Invoke(); });
        }
    }
}