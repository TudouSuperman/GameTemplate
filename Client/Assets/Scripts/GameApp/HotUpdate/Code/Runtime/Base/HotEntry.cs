using UnityEngine;
using UnityGameFramework.Runtime;

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

        public static GameApp.Hot.Procedure.ProcedureComponent Procedure { get; private set; }

        private void InitComponents()
        {
            Procedure = HotComponentEntry.GetComponent<GameApp.Hot.Procedure.ProcedureComponent>();
        }
    }
}