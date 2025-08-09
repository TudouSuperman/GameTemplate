using UnityGameFramework.Runtime;


namespace GameApp.Hotfix
{
    public sealed class DialogFormLogic : UGuiFormLogic
    {
        private DialogParams m_DialogParams;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            
            DialogFormView _view = (DialogFormView)m_UGuiFormView;
            _view.OnConfirmClicked += () =>
            {
                m_DialogParams.OnClickConfirm?.Invoke(m_DialogParams.UserData);
                GameEntry.UI.CloseUIForm(UIForm);
            };
            _view.OnCancelClicked += () =>
            {
                m_DialogParams.OnClickCancel?.Invoke(m_DialogParams.UserData);
                GameEntry.UI.CloseUIForm(UIForm);
            };
            _view.OnOtherClicked += () =>
            {
                m_DialogParams.OnClickOther?.Invoke(m_DialogParams.UserData);
                GameEntry.UI.CloseUIForm(UIForm);
            };
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            m_DialogParams = (DialogParams)userData;
            if (null == m_DialogParams)
            {
                Log.Warning("DialogParams is invalid.");
                return;
            }

            DialogFormView _view = (DialogFormView)m_UGuiFormView;
            switch (m_DialogParams.Mode)
            {
                case EDialogMode.OneButtonMode:
                    _view.ShowConfirmNode();
                    _view.HideCancelNode();
                    _view.HideOtherNode();
                    break;
                case EDialogMode.TwoButtonMode:
                    _view.ShowConfirmNode();
                    _view.ShowCancelNode();
                    _view.HideOtherNode();
                    break;
                case EDialogMode.ThreeButtonMode:
                    _view.ShowConfirmNode();
                    _view.ShowCancelNode();
                    _view.ShowOtherNode();
                    break;
            }

            _view.RefreshTitleText(m_DialogParams.Title);
            _view.RefreshMessageText(m_DialogParams.Message);
            _view.RefreshConfirmText(string.IsNullOrEmpty(m_DialogParams.ConfirmText) ? GameEntry.Localization.GetString(HotConstant.LocalizationKey.Dialog_Confirm) : m_DialogParams.ConfirmText);
            _view.RefreshCancelText(string.IsNullOrEmpty(m_DialogParams.CancelText) ? GameEntry.Localization.GetString(HotConstant.LocalizationKey.Dialog_Cancel) : m_DialogParams.CancelText);
            _view.RefreshOtherText(string.IsNullOrEmpty(m_DialogParams.OtherText) ? GameEntry.Localization.GetString(HotConstant.LocalizationKey.Dialog_Other) : m_DialogParams.OtherText);
            RefreshPauseGame(m_DialogParams.PauseGameFlag);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            
            m_DialogParams = null;
            RefreshPauseGame(false);
        }

        private void RefreshPauseGame(bool flag)
        {
            if (flag)
                GameEntry.Base.PauseGame();
            else
                GameEntry.Base.ResumeGame();
        }
    }
}