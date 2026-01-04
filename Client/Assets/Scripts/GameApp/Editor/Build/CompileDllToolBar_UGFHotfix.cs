#if UNITY_HOTFIX
using ToolbarExtension;
using UnityEngine;

namespace GameApp.Editor
{
    internal sealed class CompileDllToolBar_UGFHotfix
    {
        private static readonly GUIContent s_ButtonGUIContent = new GUIContent("Copy Hot Dll", "Copy Compile GameHotfix Dll.");

        [Toolbar(OnGUISide.Left, 50)]
        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_ButtonGUIContent))
            {
                BuildAssemblyTool.Build();
            }
        }
    }

    internal sealed class AOTDllToolBar_UGFHotfix
    {
        private static readonly GUIContent s_ButtonGUIContent = new GUIContent("Copy Aot Dll", "Copy Compile AOT Dll.");

        [Toolbar(OnGUISide.Left, 49)]
        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_ButtonGUIContent))
            {
                HybridCLREditor.CopyAotDll();
            }
        }
    }
}
#endif