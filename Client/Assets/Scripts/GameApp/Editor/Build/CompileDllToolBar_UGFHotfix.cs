#if UNITY_HOTFIX
using ToolbarExtension;
using UnityEngine;

namespace GameApp.Hotfix.Editor
{
    internal sealed class CompileDllToolBar_UGFHotfix
    {
        private static readonly GUIContent s_ButtonGUIContent = new GUIContent("CopyHotDll", "Copy Compile GameHotfix Dll.");

        [Toolbar(OnGUISide.Left, 50)]
        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_ButtonGUIContent))
            {
                BuildAssemblyTool.Build();
            }
        }
    }
}
#endif