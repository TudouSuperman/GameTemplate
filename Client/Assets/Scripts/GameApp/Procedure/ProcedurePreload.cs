using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using GameFramework.DataTable;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityGameFramework.Runtime;

namespace GameApp
{
    public class ProcedurePreload : ProcedureBase
    {
        private static readonly string[] DataTableNames = new string[]
        {
            "Asset",
            "UIFormGroup",
            "EntityGroup",
            "SoundGroup",
            "UIForm",
            "Entity",
            "UISound",
            "Music",
            "Sound",
            "Scene",
        };

        private readonly Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            GameEntry.Event.Subscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            GameEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            GameEntry.Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);

            m_LoadedFlag.Clear();

            PreloadResources();
            PreloadResourcesAsync().Forget();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
            GameEntry.Event.Unsubscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
            GameEntry.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            GameEntry.Event.Unsubscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            foreach (KeyValuePair<string, bool> loadedFlag in m_LoadedFlag)
            {
                if (!loadedFlag.Value)
                {
                    return;
                }
            }
#if UNITY_EDITOR
            Check();
#endif
            SetComponents();
            ChangeState<ProcedureGameHotfix>(procedureOwner);
        }

        private void PreloadResources()
        {
            // Preload configs
            LoadConfig("DefaultConfig");

            // Preload data tables
            foreach (string dataTableName in DataTableNames)
            {
                LoadTableData(dataTableName);
            }
        }

        private async UniTaskVoid PreloadResourcesAsync()
        {
#if UNITY_HOTFIX && ENABLE_IL2CPP
            m_LoadedFlag.Add(HybridCLRHelper.ConfigAsset, false);
            await HybridCLRHelper.LoadAsync();
            m_LoadedFlag[HybridCLRHelper.ConfigAsset] = true;
#endif
        }

        private void SetComponents()
        {
            SetUIComponent();
            SetEntityComponent();
            SetSoundComponent();

            void SetUIComponent()
            {
                IDataTable<DRUIFormGroup> _groups = GameEntry.DataTable.GetDataTable<DRUIFormGroup>();
                foreach (DRUIFormGroup _group in _groups)
                {
                    GameEntry.UI.AddUIGroup(_group.GroupName, _group.GroupDepth);
                }

                Log.Info("Init UI Group settings complete.");
            }

            void SetEntityComponent()
            {
                IDataTable<DREntityGroup> _groups = GameEntry.DataTable.GetDataTable<DREntityGroup>();
                foreach (DREntityGroup _group in _groups)
                {
                    GameEntry.Entity.AddEntityGroup(_group.GroupName, _group.InstanceAutoReleaseInterval, _group.InstanceCapacity, _group.InstanceExpireTime, _group.InstancePriority);
                }

                Log.Info("Init Entity Group settings complete.");
            }

            void SetSoundComponent()
            {
                IDataTable<DRSoundGroup> _groups = GameEntry.DataTable.GetDataTable<DRSoundGroup>();
                foreach (DRSoundGroup _group in _groups)
                {
                    GameEntry.Sound.AddSoundGroup(_group.GroupName, _group.AvoidBeingReplacedBySamePriority, _group.Mute, _group.Volume, _group.SoundAgentCount);
                    GameEntry.Sound.Mute(_group.GroupName, GameEntry.Setting.GetBool(Utility.Text.Format(Constant.Setting.Sound_Group_Muted, _group.GroupName), false));
                    GameEntry.Sound.SetVolume(_group.GroupName, GameEntry.Setting.GetFloat(Utility.Text.Format(Constant.Setting.Sound_Group_Volume, _group.GroupName), 1));
                }

                Log.Info("Init Sound Group settings complete.");
            }
        }

        private void LoadConfig(string configName)
        {
            string configAssetName = AssetPathUtility.GetConfigAsset(configName, false);
            m_LoadedFlag.Add(configAssetName, false);
            GameEntry.Config.ReadData(configAssetName, Constant.AssetPriority.Config_Asset, this);
        }

        private void LoadTableData(string dataTableName)
        {
            string dataTableAssetName = AssetPathUtility.GetTableDataAsset(dataTableName);
            GameEntry.DataTable.LoadDataTable(dataTableName, dataTableAssetName, this);
            m_LoadedFlag.Add(dataTableAssetName, false);
        }

        private void OnLoadConfigSuccess(object sender, GameEventArgs e)
        {
            LoadConfigSuccessEventArgs ne = (LoadConfigSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LoadedFlag[ne.ConfigAssetName] = true;
            Log.Info("Load config '{0}' OK.", ne.ConfigAssetName);
        }

        private void OnLoadConfigFailure(object sender, GameEventArgs e)
        {
            LoadConfigFailureEventArgs ne = (LoadConfigFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load config '{0}' from '{1}' with error message '{2}'.", ne.ConfigAssetName, ne.ConfigAssetName, ne.ErrorMessage);
        }

        private void OnLoadDataTableSuccess(object sender, GameEventArgs e)
        {
            LoadDataTableSuccessEventArgs ne = (LoadDataTableSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LoadedFlag[ne.DataTableAssetName] = true;
            Log.Info("Load data table '{0}' OK.", ne.DataTableAssetName);
        }

        private void OnLoadDataTableFailure(object sender, GameEventArgs e)
        {
            LoadDataTableFailureEventArgs ne = (LoadDataTableFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load data table '{0}' from '{1}' with error message '{2}'.", ne.DataTableAssetName, ne.DataTableAssetName, ne.ErrorMessage);
        }

#if UNITY_EDITOR
        private void Check()
        {
            IDataTable<DRAsset> _dtAsset = GameEntry.DataTable.GetDataTable<DRAsset>();
            foreach (DRAsset _drAsset in _dtAsset)
            {
                Log.Debug("<color=#00FF00>Asset Table Row : {0} {1} {2}</color>", _drAsset.Id, _drAsset.AssetName, _drAsset.AssetPath);
            }

            IDataTable<DRUIFormGroup> _dtUIFormGroup = GameEntry.DataTable.GetDataTable<DRUIFormGroup>();
            foreach (DRUIFormGroup _drUIFormGroup in _dtUIFormGroup)
            {
                Log.Debug("<color=#00FF00>UIFormGroup Table Row : {0} {1} {2}</color>", _drUIFormGroup.Id, _drUIFormGroup.GroupName, _drUIFormGroup.GroupDepth);
            }

            IDataTable<DREntityGroup> _dtEntityGroup = GameEntry.DataTable.GetDataTable<DREntityGroup>();
            foreach (DREntityGroup _drEntityGroup in _dtEntityGroup)
            {
                Log.Debug("<color=#00FF00>EntityGroup Table Row : {0} {1} {2} {3} {4} {5}</color>", _drEntityGroup.Id, _drEntityGroup.GroupName, _drEntityGroup.InstanceAutoReleaseInterval, _drEntityGroup.InstanceCapacity, _drEntityGroup.InstanceExpireTime, _drEntityGroup.InstancePriority);
            }

            IDataTable<DRSoundGroup> _dtSoundGroup = GameEntry.DataTable.GetDataTable<DRSoundGroup>();
            foreach (DRSoundGroup _drSoundGroup in _dtSoundGroup)
            {
                Log.Debug("<color=#00FF00>SoundGroup Table Row : {0} {1} {2} {3} {4} {5}</color>", _drSoundGroup.Id, _drSoundGroup.GroupName, _drSoundGroup.SoundAgentCount, _drSoundGroup.AvoidBeingReplacedBySamePriority, _drSoundGroup.Mute, _drSoundGroup.Volume);
            }

            IDataTable<DRUIForm> _dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
            foreach (DRUIForm _drUIForm in _dtUIForm)
            {
                Log.Debug("<color=#00FF00>UIForm Table Row : {0} {1} {2} {3} {4}</color>", _drUIForm.Id, _drUIForm.AssetId, _drUIForm.GroupId, _drUIForm.AllowMultiInstance, _drUIForm.PauseCoveredUIForm);
            }

            IDataTable<DREntity> _dtEntity = GameEntry.DataTable.GetDataTable<DREntity>();
            foreach (DREntity _drEntity in _dtEntity)
            {
                Log.Debug("<color=#00FF00>Entity Table Row : {0} {1} {2}</color>", _drEntity.Id, _drEntity.AssetId, _drEntity.GroupId);
            }

            IDataTable<DRUISound> _dtUISound = GameEntry.DataTable.GetDataTable<DRUISound>();
            foreach (DRUISound _drUISound in _dtUISound)
            {
                Log.Debug("<color=#00FF00>UISound Table Row : {0} {1} {2} {3} {4}</color>", _drUISound.Id, _drUISound.AssetId, _drUISound.GroupId, _drUISound.Priority, _drUISound.Volume);
            }

            IDataTable<DRMusic> _dtMusic = GameEntry.DataTable.GetDataTable<DRMusic>();
            foreach (DRMusic _drMusic in _dtMusic)
            {
                Log.Debug("<color=#00FF00>Music Table Row : {0} {1} {2}</color>", _drMusic.Id, _drMusic.AssetId, _drMusic.GroupId);
            }

            IDataTable<DRSound> _dtSound = GameEntry.DataTable.GetDataTable<DRSound>();
            foreach (DRSound _drSound in _dtSound)
            {
                Log.Debug("<color=#00FF00>UISound Table Row : {0} {1} {2} {3} {4} {5} {6} {7}</color>", _drSound.Id, _drSound.AssetId, _drSound.GroupId, _drSound.Priority, _drSound.Loop, _drSound.Volume, _drSound.SpatialBlend, _drSound.MaxDistance);
            }

            IDataTable<DRScene> _dtScene = GameEntry.DataTable.GetDataTable<DRScene>();
            foreach (DRScene _drScene in _dtScene)
            {
                Log.Debug("<color=#00FF00>Scene Table Row : {0} {1} {2}</color>", _drScene.Id, _drScene.AssetId, _drScene.BackgroundMusicId);
            }
        }
#endif
    }
}