using UnityEngine;
using UnityGameFramework.Runtime;
using GameApp.Singleton;
using GameApp.Hot.UI;
using GameApp.Hot.Procedure;

namespace GameApp.Hot
{
    public sealed class HotEntry : MonoBehaviour
    {
        /// <summary>
        /// 程序入口
        /// </summary>
        /// <returns></returns>
        private void Start()
        {
            Log.Info("GameApp.Hot.Code Start!");

            InitComponents();

            HotComponentEntry.Initialize();
        }

        private void Update()
        {
            HotComponentEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            HotComponentEntry.Shutdown();
        }

        public static HotProcedureComponent Procedure { get; private set; }
        public static ModelManager ModelManager { get; private set; }

        private void InitComponents()
        {
            Procedure = HotComponentEntry.GetComponent<HotProcedureComponent>();
            ModelManager = SingletonManager.AddSingleton<ModelManager>();
        }
    }
}