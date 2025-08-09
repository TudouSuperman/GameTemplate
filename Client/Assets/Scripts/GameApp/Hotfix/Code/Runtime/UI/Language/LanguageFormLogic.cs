using GameFramework.Localization;
using UnityGameFramework.Runtime;

namespace GameApp.Hotfix
{
    public sealed class LanguageFormLogic : UGuiFormLogicEx
    {
        private Language m_SelectedLanguage = Language.Unspecified;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            LanguageFormView _view = (LanguageFormView)m_UGuiFormView;
            _view.OnChineseChanged += OnChineseSimplifiedSelected;
            _view.OnEnglishChanged += OnEnglishSelected;
            _view.OnOkClicked += () =>
            {
                GameEntry.UI.OpenUIForm((int)EUIFormID.DialogForm, new DialogParams
                {
                    Mode = EDialogMode.TwoButtonMode,
                    Title = GameEntry.Localization.GetString(HotConstant.LocalizationKey.Language_AskQuitGame_Title),
                    Message = GameEntry.Localization.GetString(HotConstant.LocalizationKey.Language_AskQuitGame_Message),
                    OnClickConfirm = (data) =>
                    {
                        if (m_SelectedLanguage == GameEntry.Localization.Language)
                        {
                            GameEntry.UI.CloseUIForm(UIForm);
                            return;
                        }

                        GameEntry.Setting.SetString(Constant.Setting.Language, m_SelectedLanguage.ToString());
                        GameEntry.Setting.Save();
                        GameEntry.Sound.StopMusic();
                        UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Restart);
                    },
                    OnClickCancel = (data) => { GameEntry.UI.CloseUIForm(UIForm); },
                    UserData = default
                });
            };
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            LanguageFormView _view = (LanguageFormView)m_UGuiFormView;
            m_SelectedLanguage = GameEntry.Localization.Language;
            switch (m_SelectedLanguage)
            {
                case Language.ChineseSimplified:
                    _view.SetChinese(true);
                    break;
                case Language.English:
                    _view.SetEnglish(true);
                    break;
            }
        }

        private void OnChineseSimplifiedSelected(bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            m_SelectedLanguage = Language.ChineseSimplified;
        }

        private void OnEnglishSelected(bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            m_SelectedLanguage = Language.English;
        }
    }
}