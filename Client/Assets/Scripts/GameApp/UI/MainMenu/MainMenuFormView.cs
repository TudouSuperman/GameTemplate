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
            NewGameButton.onClick.AddListener(() => { OnNewGameClicked?.Invoke(); });
            LanguageButton.onClick.AddListener(() => { OnLanguageClicked?.Invoke(); });
            SettingButton.onClick.AddListener(() => { OnSettingClicked?.Invoke(); });
            ExitGameButton.onClick.AddListener(() => { OnExitGameClicked?.Invoke(); });
        }
    }
}