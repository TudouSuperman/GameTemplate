using UnityGameFramework.Runtime;

namespace GameApp.UI
{
    public sealed class DialogFormLogic : UGuiFormLogic
    {
        private DialogParams m_DialogParams;

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
            _view.OnConfirmClicked += OnConfirmCB;
            _view.OnCancelClicked += OnCancelCB;
            _view.OnOtherClicked += OnOtherCB;
            RefreshPauseGame(m_DialogParams.PauseGameFlag);
            RefreshDialogMode(m_DialogParams.Mode);
            RefreshTitleText(m_DialogParams.Title);
            RefreshMessageText(m_DialogParams.Message);
            RefreshConfirmText(m_DialogParams.ConfirmText);
            RefreshCancelText(m_DialogParams.CancelText);
            RefreshOtherText(m_DialogParams.OtherText);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);

            DialogFormView _view = (DialogFormView)m_UGuiFormView;
            _view.OnConfirmClicked -= OnConfirmCB;
            _view.OnCancelClicked -= OnCancelCB;
            _view.OnOtherClicked -= OnOtherCB;
            RefreshPauseGame(false);
            RefreshTitleText(string.Empty);
            RefreshMessageText(string.Empty);
            RefreshConfirmText(string.Empty);
            RefreshCancelText(string.Empty);
            RefreshOtherText(string.Empty);
            m_DialogParams = null;
        }

        private void OnConfirmCB()
        {
            Close();
            m_DialogParams.OnClickConfirm?.Invoke(m_DialogParams.UserData);
        }

        private void OnCancelCB()
        {
            Close();
            m_DialogParams.OnClickCancel?.Invoke(m_DialogParams.UserData);
        }

        private void OnOtherCB()
        {
            Close();
            m_DialogParams.OnClickOther?.Invoke(m_DialogParams.UserData);
        }

        private void RefreshPauseGame(bool flag)
        {
            if (flag)
            {
                GameEntry.Base.PauseGame();
            }
            else
            {
                GameEntry.Base.ResumeGame();
            }
        }

        private void RefreshDialogMode(EDialogMode dialogMode)
        {
            DialogFormView _view = (DialogFormView)m_UGuiFormView;
            switch (dialogMode)
            {
                case GameApp.EDialogMode.OneButtonMode:
                    _view.ConfirmNodeRectTransform.gameObject.SetActive(true);
                    _view.CancelNodeRectTransform.gameObject.SetActive(false);
                    _view.OtherNodeRectTransform.gameObject.SetActive(false);
                    break;
                case GameApp.EDialogMode.TwoButtonMode:
                    _view.ConfirmNodeRectTransform.gameObject.SetActive(true);
                    _view.CancelNodeRectTransform.gameObject.SetActive(true);
                    _view.OtherNodeRectTransform.gameObject.SetActive(false);
                    break;
                case GameApp.EDialogMode.ThreeButtonMode:
                    _view.ConfirmNodeRectTransform.gameObject.SetActive(true);
                    _view.CancelNodeRectTransform.gameObject.SetActive(true);
                    _view.OtherNodeRectTransform.gameObject.SetActive(true);
                    break;
            }
        }

        private void RefreshTitleText(string titleText)
        {
            DialogFormView _view = (DialogFormView)m_UGuiFormView;
            _view.TitleTextMeshProUGUI.text = titleText;
        }

        private void RefreshMessageText(string messageText)
        {
            DialogFormView _view = (DialogFormView)m_UGuiFormView;
            _view.MessageTextMeshProUGUI.text = messageText;
        }

        private void RefreshConfirmText(string confirmText)
        {
            if (string.IsNullOrEmpty(confirmText))
            {
                confirmText = GameEntry.Localization.GetString(Constant.LanguageKey.Dialog_Confirm);
            }

            DialogFormView _view = (DialogFormView)m_UGuiFormView;
            _view.ConfirmDescribeTextMeshProUGUI.text = confirmText;
        }

        private void RefreshCancelText(string cancelText)
        {
            if (string.IsNullOrEmpty(cancelText))
            {
                cancelText = GameEntry.Localization.GetString(Constant.LanguageKey.Dialog_Cancel);
            }

            DialogFormView _view = (DialogFormView)m_UGuiFormView;
            _view.CancelDescribeTextMeshProUGUI.text = cancelText;
        }

        private void RefreshOtherText(string otherText)
        {
            if (string.IsNullOrEmpty(otherText))
            {
                otherText = GameEntry.Localization.GetString(Constant.LanguageKey.Dialog_Other);
            }

            DialogFormView _view = (DialogFormView)m_UGuiFormView;
            _view.OtherDescribeTextMeshProUGUI.text = otherText;
        }
    }
}