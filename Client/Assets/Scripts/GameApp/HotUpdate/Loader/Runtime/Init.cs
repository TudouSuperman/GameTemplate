using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GameFramework;
using UnityGameFramework.Extension;

namespace GameApp.Hot
{
    [DisallowMultipleComponent]
    public sealed class Init : MonoBehaviour
    {
        public static Init Instance { get; private set; }

        private GameObject m_HotEntryAsset;
        private GameObject m_HotEntryGameObject;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            StartAsync().Forget();
        }

        private void OnDestroy()
        {
            if (m_HotEntryGameObject != null)
            {
                DestroyImmediate(m_HotEntryGameObject);
                m_HotEntryGameObject = null;
            }

            if (m_HotEntryAsset != null)
            {
                GameEntry.Resource.UnloadAsset(m_HotEntryAsset);
                m_HotEntryAsset = null;
            }
        }

        private async UniTaskVoid StartAsync()
        {
            // AppDomain.CurrentDomain.UnhandledException += (sender, e) => { Log.Error(e.ExceptionObject.ToString()); };
            if (Define.EnableHotfix && GameEntry.CodeRunner.EnableCodeBytesMode)
            {
                byte[] _dllBytes = await LoadCodeBytesAsync("GameApp.Hot.Runtime.dll.bytes");
                byte[] _pdbBytes = await LoadCodeBytesAsync("GameApp.Hot.Runtime.pdb.bytes");
                Assembly.Load(_dllBytes, _pdbBytes);
            }

            m_HotEntryAsset = await GameEntry.Resource.LoadAssetAsync<GameObject>(AssetPathUtility.GetGameHotAsset("HotEntry.prefab"));
            m_HotEntryGameObject = Instantiate(m_HotEntryAsset, GameEntry.CodeRunner.transform);
        }

        private async UniTask<byte[]> LoadCodeBytesAsync(string fileName)
        {
            fileName = AssetPathUtility.GetGameHotAsset(Utility.Text.Format("Code/{0}", fileName));
            TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(fileName);
            byte[] bytes = textAsset.bytes;
            GameEntry.Resource.UnloadAsset(textAsset);
            return bytes;
        }
    }
}