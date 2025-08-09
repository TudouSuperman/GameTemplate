namespace GameApp.Hotfix
{
    public sealed class SettingFormLogic : UGuiFormLogicEx
    {
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            SettingFormView _view = (SettingFormView)m_UGuiFormView;
            _view.OnMusicMuteChanged += OnMusicMuteChanged;
            _view.OnMusicVolumeChanged += OnMusicVolumeChanged;
            _view.OnSoundMuteChanged += OnSoundMuteChanged;
            _view.OnSoundVolumeChanged += OnSoundVolumeChanged;
            _view.OnUISoundMuteChanged += OnUISoundMuteChanged;
            _view.OnUISoundVolumeChanged += OnUISoundVolumeChanged;
            _view.OnOkClicked += () =>
            {
                GameEntry.UI.OpenUIForm((int)EUIFormID.MainMenuForm);
                GameEntry.UI.CloseUIForm(UIForm);
            };
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            SettingFormView _view = (SettingFormView)m_UGuiFormView;
            _view.SetMusic(!GameEntry.Sound.IsMuted(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.Music))));
            _view.SetMusicVolume(GameEntry.Sound.GetVolume(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.Music))));
            _view.SetSound(!GameEntry.Sound.IsMuted(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.Sound))));
            _view.SetSoundVolume(GameEntry.Sound.GetVolume(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.Sound))));
            _view.SetUISound(!GameEntry.Sound.IsMuted(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.UISound))));
            _view.SetUISoundVolume(GameEntry.Sound.GetVolume(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.UISound))));
        }

        private void OnMusicMuteChanged(bool isOn) => GameEntry.Sound.Mute(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.Music)), !isOn);
        private void OnMusicVolumeChanged(float volume) => GameEntry.Sound.SetVolume(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.Music)), volume);
        private void OnSoundMuteChanged(bool isOn) => GameEntry.Sound.Mute(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.Sound)), !isOn);
        private void OnSoundVolumeChanged(float volume) => GameEntry.Sound.SetVolume(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.Sound)), volume);
        private void OnUISoundMuteChanged(bool isOn) => GameEntry.Sound.Mute(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.UISound)), !isOn);
        private void OnUISoundVolumeChanged(float volume) => GameEntry.Sound.SetVolume(GameFramework.Utility.Text.Format("{0}", nameof(ESoundGroupID.UISound)), volume);
    }
}