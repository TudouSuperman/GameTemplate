using UnityEngine;
using GameApp.Singleton;
using GameApp.UI;

namespace GameApp
{
    public partial class GameEntry
    {
        public static ModelManager ModelManager
        {
            get; 
            private set;
        }

        private void Update()
        {
            SingletonManager.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
            SingletonManager.LateUpdate();
        }

        private static void InitSingletons()
        {
            ModelManager = SingletonManager.AddSingleton<ModelManager>();
        }

        private static void ClearSingletons() => SingletonManager.Clear();
    }
}