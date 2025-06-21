using ToolbarExtension;
using UnityEditor;
using UnityEngine;

namespace GameApp.Editor
{
    public static class FocusFolderToolBar
    {
        private static readonly GUIContent s_UIResFocusGUIContent = new GUIContent("UI-Res", "Focus UI Res Folder.");

        [Toolbar(OnGUISide.Left, -1)]
        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_UIResFocusGUIContent))
            {
                EditorUtility.FocusProjectWindow();
                Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Res/Artwork/UI");
                Selection.activeObject = obj;
            }
        }
    }
}
