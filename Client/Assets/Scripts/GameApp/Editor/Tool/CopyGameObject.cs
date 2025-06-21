using UnityEditor;
using UnityEngine;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameApp.Editor
{
    public static class CopyGameObject
    {
        [MenuItem("GameObject/Copy Full Path", false, 0)]
        private static void CopyFullPath()
        {
            if (Selection.activeGameObject == null)
            {
                Debug.LogWarning("No GameObject selected!");
                return;
            }

            string fullPath = GetGameObjectPath(Selection.activeGameObject);
            EditorGUIUtility.systemCopyBuffer = fullPath;
            Log.Debug(Utility.Text.Format("Copied path: {0}", fullPath));

            string GetGameObjectPath(GameObject obj)
            {
                string path = obj.name;
                while (obj.transform.parent != null)
                {
                    obj = obj.transform.parent.gameObject;
                    path = obj.name + "/" + path;
                }
                return path;
            }
        }
    }
}