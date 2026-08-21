using UnityEditor;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameApp.Editor
{
    internal static class GLogTool
    {
        [InitializeOnLoadMethod]
        private static void OnInitialize()
        {
            GameFrameworkLog.SetLogHelper(new DefaultLogHelper());

            GLog.Entity.IsEnabled = EditorPrefs.GetBool("GameApp.Log.Entity", false);
            GLog.Game.IsEnabled = EditorPrefs.GetBool("GameApp.Log.Game", false);
            GLog.Procedure.IsEnabled = EditorPrefs.GetBool("GameApp.Log.Procedure", false);
            GLog.Resource.IsEnabled = EditorPrefs.GetBool("GameApp.Log.Resource", false);
            GLog.Scene.IsEnabled = EditorPrefs.GetBool("GameApp.Log.Scene", false);
            GLog.Sound.IsEnabled = EditorPrefs.GetBool("GameApp.Log.Sound", false);
            GLog.UI.IsEnabled = EditorPrefs.GetBool("GameApp.Log.UI", false);

            Menu.SetChecked("GameApp/Log Tool/Entity", GLog.Entity.IsEnabled);
            Menu.SetChecked("GameApp/Log Tool/Game", GLog.Game.IsEnabled);
            Menu.SetChecked("GameApp/Log Tool/Procedure", GLog.Procedure.IsEnabled);
            Menu.SetChecked("GameApp/Log Tool/Resource", GLog.Resource.IsEnabled);
            Menu.SetChecked("GameApp/Log Tool/Scene", GLog.Scene.IsEnabled);
            Menu.SetChecked("GameApp/Log Tool/Sound", GLog.Sound.IsEnabled);
            Menu.SetChecked("GameApp/Log Tool/UI", GLog.UI.IsEnabled);
        }

        [MenuItem("GameApp/Log Tool/Enable All", priority = -2)]
        private static void EnableAllLog()
        {
            SetAllLog(true);
        }

        [MenuItem("GameApp/Log Tool/Disable All", priority = -1)]
        private static void DisableAllLog()
        {
            SetAllLog(false);
        }

        private static void SetAllLog(bool isEnabled)
        {
            EditorPrefs.SetBool("GameApp.Log.Entity", isEnabled);
            EditorPrefs.SetBool("GameApp.Log.Game", isEnabled);
            EditorPrefs.SetBool("GameApp.Log.Procedure", isEnabled);
            EditorPrefs.SetBool("GameApp.Log.Resource", isEnabled);
            EditorPrefs.SetBool("GameApp.Log.Scene", isEnabled);
            EditorPrefs.SetBool("GameApp.Log.Entity", isEnabled);
            EditorPrefs.SetBool("GameApp.Log.UI", isEnabled);

            GLog.Entity.IsEnabled = isEnabled;
            GLog.Game.IsEnabled = isEnabled;
            GLog.Procedure.IsEnabled = isEnabled;
            GLog.Resource.IsEnabled = isEnabled;
            GLog.Scene.IsEnabled = isEnabled;
            GLog.Sound.IsEnabled = isEnabled;
            GLog.UI.IsEnabled = isEnabled;

            Menu.SetChecked("GameApp/Log Tool/Entity", isEnabled);
            Menu.SetChecked("GameApp/Log Tool/Game", isEnabled);
            Menu.SetChecked("GameApp/Log Tool/Procedure", isEnabled);
            Menu.SetChecked("GameApp/Log Tool/Resource", isEnabled);
            Menu.SetChecked("GameApp/Log Tool/Scene", isEnabled);
            Menu.SetChecked("GameApp/Log Tool/Sound", isEnabled);
            Menu.SetChecked("GameApp/Log Tool/UI", isEnabled);
        }

        [MenuItem("GameApp/Log Tool/Entity")]
        private static void ToggleEntityLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("GameApp.Log.Entity", false);
            EditorPrefs.SetBool("GameApp.Log.Entity", isEnabled);
            GLog.Entity.IsEnabled = isEnabled;
            Menu.SetChecked("GameApp/Log Tool/Entity", isEnabled);
        }

        [MenuItem("GameApp/Log Tool/Game")]
        private static void ToggleGameLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("GameApp.Log.Game", false);
            EditorPrefs.SetBool("GameApp.Log.Game", isEnabled);
            GLog.Game.IsEnabled = isEnabled;
            Menu.SetChecked("GameApp/Log Tool/Game", isEnabled);
        }

        [MenuItem("GameApp/Log Tool/Procedure")]
        private static void ToggleProcedureLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("GameApp.Log.Procedure", false);
            EditorPrefs.SetBool("GameApp.Log.Procedure", isEnabled);
            GLog.Procedure.IsEnabled = isEnabled;
            Menu.SetChecked("GameApp/Log Tool/Procedure", isEnabled);
        }

        [MenuItem("GameApp/Log Tool/Resource")]
        private static void ToggleResourceLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("GameApp.Log.Resource", false);
            EditorPrefs.SetBool("GameApp.Log.Resource", isEnabled);
            GLog.Resource.IsEnabled = isEnabled;
            Menu.SetChecked("GameApp/Log Tool/Resource", isEnabled);
        }

        [MenuItem("GameApp/Log Tool/Scene")]
        private static void ToggleSceneLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("GameApp.Log.Scene", false);
            EditorPrefs.SetBool("GameApp.Log.Scene", isEnabled);
            GLog.Scene.IsEnabled = isEnabled;
            Menu.SetChecked("GameApp/Log Tool/Scene", isEnabled);
        }

        [MenuItem("GameApp/Log Tool/Sound")]
        private static void ToggleSoundLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("GameApp.Log.Sound", false);
            EditorPrefs.SetBool("GameApp.Log.Sound", isEnabled);
            GLog.Sound.IsEnabled = isEnabled;
            Menu.SetChecked("GameApp/Log Tool/Sound", isEnabled);
        }

        [MenuItem("GameApp/Log Tool/UI")]
        private static void ToggleUILog()
        {
            bool isEnabled = !EditorPrefs.GetBool("GameApp.Log.UI", false);
            EditorPrefs.SetBool("GameApp.Log.UI", isEnabled);
            GLog.UI.IsEnabled = isEnabled;
            Menu.SetChecked("GameApp/Log Tool/UI", isEnabled);
        }
    }
}