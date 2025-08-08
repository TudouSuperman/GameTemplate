using UnityGameFramework.Runtime;
using GameApp.UI;

namespace GameApp.Hotfix.UI
{
    public sealed class MainMenuFormLogic : UGuiFormLogic
    {
        private Procedure.ProcedureGame m_ProcedureGame;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            MainMenuFormView _view = (MainMenuFormView)m_UGuiFormView;
            _view.OnNewGameClicked += () => m_ProcedureGame.StartGame();
            _view.OnSettingClicked += () => GameEntry.UI.OpenUIForm((int)EUIFormID.SettingForm);
            _view.OnLanguageClicked += () => GameEntry.UI.OpenUIForm((int)EUIFormID.LanguageForm);
            _view.OnExitGameClicked += () => UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_ProcedureGame = (Procedure.ProcedureGame)userData;
            if (m_ProcedureGame == null)
            {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);

            m_ProcedureGame = null;
        }
    }
}