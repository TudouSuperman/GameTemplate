using GameApp.UI;

namespace GameApp.Hot.UI
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
                GameEntry.UI.OpenUIForm((int)EUIFormID.MainMenuForm);
                GameEntry.UI.CloseUIForm(UIForm);
            };
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            SettingFormView _view = (SettingFormView)m_UGuiFormView;
            _view.SetMusic(!GameEntry.Sound.IsMuted("Music"));
            _view.SetMusicVolume(GameEntry.Sound.GetVolume("Music"));
            _view.SetSound(!GameEntry.Sound.IsMuted("Sound"));
            _view.SetSoundVolume(GameEntry.Sound.GetVolume("Sound"));
            _view.SetUISound(!GameEntry.Sound.IsMuted("UISound"));
            _view.SetUISoundVolume(GameEntry.Sound.GetVolume("UISound"));
        }

        private void OnMusicMuteChanged(bool isOn) => GameEntry.Sound.Mute("Music", !isOn);
        private void OnMusicVolumeChanged(float volume) => GameEntry.Sound.SetVolume("Music", volume);
        private void OnSoundMuteChanged(bool isOn) => GameEntry.Sound.Mute("Sound", !isOn);
        private void OnSoundVolumeChanged(float volume) => GameEntry.Sound.SetVolume("Sound", volume);
        private void OnUISoundMuteChanged(bool isOn) => GameEntry.Sound.Mute("UISound", !isOn);
        private void OnUISoundVolumeChanged(float volume) => GameEntry.Sound.SetVolume("UISound", volume);
    }
}