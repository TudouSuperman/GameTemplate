using UnityEngine;
using UnityGameFramework.Runtime;
using GameApp.Hot.Procedure;
using GameApp.Hot.Model;

namespace GameApp.Hot
{
    public sealed class HotEntry : MonoBehaviour
    {
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
        public static ModelComponent Model { get; private set; }

        private void InitComponents()
        {
            Procedure = HotComponentEntry.GetComponent<HotProcedureComponent>();
            Model = HotComponentEntry.GetComponent<ModelComponent>();
        }
    }
}