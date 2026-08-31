using GameFramework.Event;
using HotfixProcedureOwner = GameFramework.Fsm.IFsm<GameApp.Hotfix.HotfixProcedureComponent>;
using UnityGameFramework.Runtime;

namespace GameApp.Hotfix
{
    public sealed class ProcedureGame : ProcedureBase
    {
        private bool m_StartGame = false;
        private MainMenuFormLogic m_MenuForm = null;

        public void StartGame()
        {
            m_StartGame = true;
        }

        protected override void OnEnter(HotfixProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            m_StartGame = false;
            GameEntry.UI.OpenUIForm((int)EUIFormID.MainMenuForm, this);

            GameEntry.Entity.ShowHostEntity(HostEntityData.Create(GameEntry.Entity.GenerateSerialId(), (int)EEntityID.HostEntity));

            GameEntry.NetworkService.InitServiceNetworkHelper(new NetworkServiceHelper());
            GameEntry.NetworkService.Connect(this);
            
            GameEntry.Timer.AddOnceTimer(3000,() =>
            {
                CSHello _csHello = GameFramework.ReferencePool.Acquire<CSHello>();
                _csHello.Text = "定时发送给服务端";
                GameEntry.NetworkService.Send(_csHello);
            });
        }

        protected override void OnLeave(HotfixProcedureOwner procedureOwner, bool isShutdown)
        {
            if (!isShutdown)
            {
                GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

                if (m_MenuForm != null)
                {
                    GameEntry.UI.CloseUIForm(m_MenuForm.UIForm);
                    m_MenuForm = null;
                }
            }

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(HotfixProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
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