using CodeBind;
using GameFramework;

namespace GameApp.UI
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
            MusicToggle.onValueChanged.AddListener((isOn) => { OnMusicMuteChanged?.Invoke(isOn); });
            MusicVolumeSlider.onValueChanged.AddListener((volume) => { OnMusicVolumeChanged?.Invoke(volume); });
            SoundToggle.onValueChanged.AddListener((isOn) => { OnSoundMuteChanged?.Invoke(isOn); });
            SoundVolumeSlider.onValueChanged.AddListener((volume) => { OnSoundVolumeChanged?.Invoke(volume); });
            UISoundToggle.onValueChanged.AddListener((isOn) => { OnUISoundMuteChanged?.Invoke(isOn); });
            UISoundVolumeSlider.onValueChanged.AddListener((volume) => { OnUISoundVolumeChanged?.Invoke(volume); });
            OKButton.onClick.AddListener(() => { OnOkClicked?.Invoke(); });
        }
    }
}