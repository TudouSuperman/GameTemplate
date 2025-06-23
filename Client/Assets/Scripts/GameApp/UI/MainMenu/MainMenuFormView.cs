using GameFramework;
using CodeBind;

namespace GameApp.UI
{
    [MonoCodeBind('-')]
    public sealed partial class MainMenuFormView : UGuiFormView
    {
        public event GameFrameworkAction OnNewGameClicked;
        public event GameFrameworkAction OnLanguageClicked;
        public event GameFrameworkAction OnSettingClicked;
        public event GameFrameworkAction OnExitGameClicked;

        public override void OnInit()
        {
            m_NewGameButton.onClick.AddListener(() => { OnNewGameClicked?.Invoke(); });
            m_LanguageButton.onClick.AddListener(() => { OnLanguageClicked?.Invoke(); });
            m_SettingButton.onClick.AddListener(() => { OnSettingClicked?.Invoke(); });
            m_ExitGameButton.onClick.AddListener(() => { OnExitGameClicked?.Invoke(); });
        }
    }
}