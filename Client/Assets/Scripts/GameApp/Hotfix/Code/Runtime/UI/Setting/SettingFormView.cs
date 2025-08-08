using GameFramework;
using CodeBind;

namespace GameApp.Hotfix
{
    [MonoCodeBind('-')]
    public sealed partial class SettingFormView : UGuiFormView
    {
        public event GameFrameworkAction<bool> OnMusicMuteChanged;
        public event GameFrameworkAction<float> OnMusicVolumeChanged;
        public event GameFrameworkAction<bool> OnSoundMuteChanged;
        public event GameFrameworkAction<float> OnSoundVolumeChanged;
        public event GameFrameworkAction<bool> OnUISoundMuteChanged;
        public event GameFrameworkAction<float> OnUISoundVolumeChanged;
        public event GameFrameworkAction OnOkClicked;

        public override void OnInit()
        {
            m_MusicToggle.onValueChanged.AddListener((isOn) => { OnMusicMuteChanged?.Invoke(isOn); });
            m_MusicVolumeSlider.onValueChanged.AddListener((volume) => { OnMusicVolumeChanged?.Invoke(volume); });
            m_SoundToggle.onValueChanged.AddListener((isOn) => { OnSoundMuteChanged?.Invoke(isOn); });
            m_SoundVolumeSlider.onValueChanged.AddListener((volume) => { OnSoundVolumeChanged?.Invoke(volume); });
            m_UISoundToggle.onValueChanged.AddListener((isOn) => { OnUISoundMuteChanged?.Invoke(isOn); });
            m_UISoundVolumeSlider.onValueChanged.AddListener((volume) => { OnUISoundVolumeChanged?.Invoke(volume); });
            m_OKButton.onClick.AddListener(() => { OnOkClicked?.Invoke(); });
        }

        public void SetMusic(bool flag) => m_MusicToggle.isOn = flag;
        public void SetMusicVolume(float value) => m_MusicVolumeSlider.value = value;
        public void SetSound(bool flag) => m_SoundToggle.isOn = flag;
        public void SetSoundVolume(float value) => m_SoundVolumeSlider.value = value;
        public void SetUISound(bool flag) => m_UISoundToggle.isOn = flag;
        public void SetUISoundVolume(float value) => m_UISoundVolumeSlider.value = value;
    }
}