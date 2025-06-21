namespace GameApp.UI
{
    public sealed class SettingFormLogic : UGuiFormLogic
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
                GameEntry.UI.OpenUIForm(EUIFormID.MainMenuForm);
                GameEntry.UI.CloseUIForm(this);
            };
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            SettingFormView _view = (SettingFormView)m_UGuiFormView;
            _view.MusicToggle.isOn = !GameEntry.Sound.IsMuted("Music");
            _view.MusicVolumeSlider.value = GameEntry.Sound.GetVolume("Music");
            _view.SoundToggle.isOn = !GameEntry.Sound.IsMuted("Sound");
            _view.SoundVolumeSlider.value = GameEntry.Sound.GetVolume("Sound");
            _view.UISoundToggle.isOn = !GameEntry.Sound.IsMuted("UISound");
            _view.UISoundVolumeSlider.value = GameEntry.Sound.GetVolume("UISound");
        }

        private void OnMusicMuteChanged(bool isOn) => GameEntry.Sound.Mute("Music", !isOn);
        private void OnMusicVolumeChanged(float volume) => GameEntry.Sound.SetVolume("Music", volume);
        private void OnSoundMuteChanged(bool isOn) => GameEntry.Sound.Mute("Sound", !isOn);
        private void OnSoundVolumeChanged(float volume) => GameEntry.Sound.SetVolume("Sound", volume);
        private void OnUISoundMuteChanged(bool isOn) => GameEntry.Sound.Mute("UISound", !isOn);
        private void OnUISoundVolumeChanged(float volume) => GameEntry.Sound.SetVolume("UISound", volume);
    }
}