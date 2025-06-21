using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace GameApp.Editor
{
    public static class OpenFolderTool
    {
        [MenuItem("GameApp/Open Folder/打开Excel文件夹", false, -99)]
        public static void OpenExcelPath()
        {
            SafeOpenFolder($"{Application.dataPath}/../../ClientExcel");
        }
        
        [MenuItem("GameApp/Open Folder/打开Proto文件夹", false, -98)]
        public static void OpenProtoPath()
        {
            SafeOpenFolder($"{Application.dataPath}/../../Proto");
        }
        
        [MenuItem("GameApp/Open Folder/打开Build文件夹", false, -97)]
        public static void OpenBuildPath()
        {
            SafeOpenFolder($"{Application.dataPath}/../../ClientBuild");
        }

        public static void SafeOpenFolder(string folderPath)
        {
            folderPath = Path.GetFullPath(folderPath);
            if (Directory.Exists(folderPath))
            {
                OpenFolder.Execute(folderPath);
            }
            else
            {
                Debug.LogError($"Open folder fail! {folderPath} not exist!");
            }
        }
    }
}
