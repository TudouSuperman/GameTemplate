using UnityEngine;
using UnityGameFramework.Runtime;
using GameApp.Hotfix.Procedure;
using GameApp.Hotfix.Model;

namespace GameApp.Hotfix
{
    public sealed class HotfixEntry : MonoBehaviour
    {
        private void Start()
        {
            Log.Info("GameApp.Hotfix.Code Start!");

            InitComponents();

            HotfixComponentEntry.Initialize();
        }

        private void Update()
        {
            HotfixComponentEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            HotfixComponentEntry.Shutdown();
        }

        public static HotfixProcedureComponent Procedure { get; private set; }
        public static ModelComponent Model { get; private set; }

        private void InitComponents()
        {
            Procedure = HotfixComponentEntry.GetComponent<HotfixProcedureComponent>();
            Model = HotfixComponentEntry.GetComponent<ModelComponent>();
        }
    }
}