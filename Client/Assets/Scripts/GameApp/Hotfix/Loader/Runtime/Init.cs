using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GameFramework;
using UnityGameFramework.Extension;

namespace GameApp.Hotfix
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
                byte[] _dllBytes = await LoadCodeBytesAsync("GameApp.Hotfix.Runtime.dll.bytes");
                byte[] _pdbBytes = await LoadCodeBytesAsync("GameApp.Hotfix.Runtime.pdb.bytes");
                // TODO 作者：
                // GameFramework 的 Shutdown 重启功能，并没有退出 App 只是再 App 里重启走了一遍框架，
                // 但是这个加载到内存里的热更程序集没有卸载导致出现重复加载程序集到内存的报错。
                // 因为在游戏过程中涉及到突然通知的热更，需要重复加载热更程序集，只能考虑退出游戏让玩家重启，或者调用相关代码重启了。
                Assembly.Load(_dllBytes, _pdbBytes);
            }

            m_HotEntryAsset = await GameEntry.Resource.LoadAssetAsync<GameObject>(AssetPathUtility.GetHotfixGameAsset("HotEntry.prefab"));
            m_HotEntryGameObject = Instantiate(m_HotEntryAsset, GameEntry.CodeRunner.transform);
        }

        private async UniTask<byte[]> LoadCodeBytesAsync(string fileName)
        {
            fileName = AssetPathUtility.GetHotfixGameAsset(Utility.Text.Format("Code/{0}", fileName));
            TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(fileName);
            byte[] bytes = textAsset.bytes;
            GameEntry.Resource.UnloadAsset(textAsset);
            return bytes;
        }
    }
}