using System.Collections.Generic;
using GameFramework;
using GameFramework.DataTable;
using GameFramework.Resource;
using GameFramework.Event;
using HotfixProcedureOwner = GameFramework.Fsm.IFsm<GameApp.Hotfix.HotfixProcedureComponent>;
using UnityGameFramework.Runtime;
using TMPro;

namespace GameApp.Hotfix
{
    public sealed class ProcedurePreload : ProcedureBase
    {
        private static readonly string[] DataTableNames = new string[]
        {
            "Guide",
        };

        private readonly Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();

        protected override void OnEnter(HotfixProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            GameEntry.Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
            GameEntry.Event.Subscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            GameEntry.Event.Subscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);

            m_LoadedFlag.Clear();

            PreloadResources();
        }

        protected override void OnLeave(HotfixProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            GameEntry.Event.Unsubscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
            GameEntry.Event.Unsubscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            GameEntry.Event.Unsubscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(HotfixProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
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
            IDataTable<DRGuide> _dtGuide = GameEntry.DataTable.GetDataTable<DRGuide>();
            foreach (DRGuide _drGuide in _dtGuide)
            {
                Log.Debug("<color=#00FF00>Guide Table Row : {0} {1} {2}</color>", _drGuide.Id, _drGuide.StepName, _drGuide.NextStepId);
            }
#endif
            ChangeState<ProcedureGame>(procedureOwner);
        }

        private void PreloadResources()
        {
            // Preload data tables
            foreach (string dataTableName in DataTableNames)
            {
                LoadTableData(dataTableName);
            }

            // Preload dictionaries
            LoadDictionary(GameEntry.Localization.Language.ToString());

            // Preload fonts
            LoadFont("HarmonyOS_SansSC_Black SDF");
        }

        private void LoadDictionary(string dictionaryName)
        {
            string dictionaryAssetName = AssetHotfixPathUtility.GetDictionaryAsset(dictionaryName, false);
            m_LoadedFlag.Add(dictionaryAssetName, false);
            GameEntry.Localization.ReadData(dictionaryAssetName, this);
        }

        private void LoadFont(string fontName)
        {
            m_LoadedFlag.Add(Utility.Text.Format("Font.{0}", fontName), false);
            GameEntry.Resource.LoadAsset
            (
                AssetHotfixPathUtility.GetFontAsset(fontName),
                Constant.AssetPriority.Font_Asset,
                new LoadAssetCallbacks
                (
                    (assetName, asset, duration, userData) =>
                    {
                        m_LoadedFlag[Utility.Text.Format("Font.{0}", fontName)] = true;
                        UGuiFormLogic.SetMainFont((TMP_FontAsset)asset);
                        Log.Info("Load font '{0}' OK.", fontName);
                    },
                    (assetName, status, errorMessage, userData) => { Log.Error("Can not load font '{0}' from '{1}' with error message '{2}'.", fontName, assetName, errorMessage); }
                )
            );
        }

        private void LoadTableData(string dataTableName)
        {
            string dataTableAssetName = AssetHotfixPathUtility.GetHotfixTableDataAsset(dataTableName);
            GameEntry.DataTable.LoadDataTableHotfix(dataTableName, dataTableAssetName, this);
            m_LoadedFlag.Add(dataTableAssetName, false);
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

        private void OnLoadDictionarySuccess(object sender, GameEventArgs e)
        {
            LoadDictionarySuccessEventArgs ne = (LoadDictionarySuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LoadedFlag[ne.DictionaryAssetName] = true;
            Log.Info("Load dictionary '{0}' OK.", ne.DictionaryAssetName);
        }

        private void OnLoadDictionaryFailure(object sender, GameEventArgs e)
        {
            LoadDictionaryFailureEventArgs ne = (LoadDictionaryFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load dictionary '{0}' from '{1}' with error message '{2}'.", ne.DictionaryAssetName, ne.DictionaryAssetName, ne.ErrorMessage);
        }
    }
}