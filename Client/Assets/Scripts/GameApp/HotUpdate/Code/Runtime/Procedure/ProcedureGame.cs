using GameFramework.Event;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameApp.Hot.Procedure.ProcedureComponent>;
using UnityGameFramework.Runtime;
using GameApp.UI;
using GameApp.Hot.UI;

namespace GameApp.Hot.Procedure
{
    public sealed class ProcedureGame : ProcedureBase
    {
        private bool m_StartGame = false;
        private MainMenuFormLogic m_MenuForm = null;

        public void StartGame()
        {
            m_StartGame = true;
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            m_StartGame = false;
            GameEntry.UI.OpenUIForm((int)EUIFormID.MainMenuForm, this);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            if (m_MenuForm != null)
            {
                GameEntry.UI.CloseUIForm(m_MenuForm.UIForm);
                m_MenuForm = null;
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_StartGame)
            {
                // 开始游戏。
            }
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_MenuForm = (MainMenuFormLogic)ne.UIForm.Logic;
        }
    }
}