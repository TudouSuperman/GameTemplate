using GameFramework;
using GameFramework.DataTable;
using GameFramework.Sound;
using UnityGameFramework.Runtime;
using GameApp.Entity;
using GameApp.DataTable;

namespace GameApp
{
    public static class SoundExtension
    {
        private const float FadeVolumeDuration = 1f;
        private static int? s_MusicSerialId = null;

        public static int? PlayMusic(this SoundComponent soundComponent, int musicId, object userData = null)
        {
            soundComponent.StopMusic();

            DRMusic drMusic = GameEntry.DataTable.GetDataRow<DRMusic>(musicId);
            if (drMusic == null)
            {
                return null;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drMusic.AssetId);
            if (drAsset == null)
            {
                return null;
            }

            DRSoundGroup drSoundGroup = GameEntry.DataTable.GetDataRow<DRSoundGroup>(drMusic.SoundGroupId);
            if (drSoundGroup == null)
            {
                return null;
            }

            PlaySoundParams playSoundParams = PlaySoundParams.Create();
            playSoundParams.Priority = 64;
            playSoundParams.Loop = true;
            playSoundParams.VolumeInSoundGroup = 1f;
            playSoundParams.FadeInSeconds = FadeVolumeDuration;
            playSoundParams.SpatialBlend = 0f;
            s_MusicSerialId = soundComponent.PlaySound(drAsset.AssetPath, drSoundGroup.GroupName, Constant.AssetPriority.Music_Asset, playSoundParams, null, userData);
            return s_MusicSerialId;
        }

        public static void StopMusic(this SoundComponent soundComponent)
        {
            if (!s_MusicSerialId.HasValue)
            {
                return;
            }

            soundComponent.StopSound(s_MusicSerialId.Value, FadeVolumeDuration);
            s_MusicSerialId = null;
        }

        public static int? PlaySound(this SoundComponent soundComponent, int soundId, BaseEntityLogic bindingEntity = null, object userData = null)
        {
            DRSound drSound = GameEntry.DataTable.GetDataRow<DRSound>(soundId);
            if (drSound == null)
            {
                return null;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drSound.AssetId);
            if (drAsset == null)
            {
                return null;
            }

            DRSoundGroup drSoundGroup = GameEntry.DataTable.GetDataRow<DRSoundGroup>(drSound.SoundGroupId);
            if (drSoundGroup == null)
            {
                return null;
            }

            PlaySoundParams playSoundParams = PlaySoundParams.Create();
            playSoundParams.Priority = drSound.Priority;
            playSoundParams.Loop = drSound.Loop;
            playSoundParams.VolumeInSoundGroup = drSound.Volume;
            playSoundParams.SpatialBlend = drSound.SpatialBlend;
            return soundComponent.PlaySound(drAsset.AssetPath, drSoundGroup.GroupName, Constant.AssetPriority.Sound_Asset, playSoundParams, bindingEntity != null ? bindingEntity.Entity : null, userData);
        }

        public static int? PlayUISound(this SoundComponent soundComponent, int uiSoundId, object userData = null)
        {
            DRUISound drUISound = GameEntry.DataTable.GetDataRow<DRUISound>(uiSoundId);
            if (drUISound == null)
            {
                return null;
            }

            DRAsset drAsset = GameEntry.DataTable.GetDataRow<DRAsset>(drUISound.AssetId);
            if (drAsset == null)
            {
                return null;
            }

            DRSoundGroup drSoundGroup = GameEntry.DataTable.GetDataRow<DRSoundGroup>(drUISound.SoundGroupId);
            if (drSoundGroup == null)
            {
                return null;
            }

            PlaySoundParams playSoundParams = PlaySoundParams.Create();
            playSoundParams.Priority = drUISound.Priority;
            playSoundParams.Loop = false;
            playSoundParams.VolumeInSoundGroup = drUISound.Volume;
            playSoundParams.SpatialBlend = 0f;
            return soundComponent.PlaySound(drAsset.AssetPath, drSoundGroup.GroupName, Constant.AssetPriority.UISound_Asset, playSoundParams, userData);
        }

        public static bool IsMuted(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return true;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return true;
            }

            return soundGroup.Mute;
        }

        public static void Mute(this SoundComponent soundComponent, string soundGroupName, bool mute)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return;
            }

            soundGroup.Mute = mute;

            GameEntry.Setting.SetBool(Utility.Text.Format(Constant.Setting.Sound_Group_Muted, soundGroupName), mute);
            GameEntry.Setting.Save();
        }

        public static float GetVolume(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return 0f;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return 0f;
            }

            return soundGroup.Volume;
        }

        public static void SetVolume(this SoundComponent soundComponent, string soundGroupName, float volume)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return;
            }

            soundGroup.Volume = volume;

            GameEntry.Setting.SetFloat(Utility.Text.Format(Constant.Setting.Sound_Group_Volume, soundGroupName), volume);
            GameEntry.Setting.Save();
        }
    }
}