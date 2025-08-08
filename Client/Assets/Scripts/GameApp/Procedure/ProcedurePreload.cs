using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using GameFramework.DataTable;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityGameFramework.Runtime;
using GameApp.DataTable;

namespace GameApp.Procedure
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
                LoaTableData(dataTableName);
            }
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

        private void LoaTableData(string dataTableName)
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
    }
}