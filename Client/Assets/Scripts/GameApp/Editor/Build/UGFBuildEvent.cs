using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Extension;
using UnityGameFramework.Extension.Editor;

namespace GameApp.Editor
{
    public static class UGFBuildEvent
    {
        [UGFBuildOnOutputUpdatableVersionListData(1)]
        public static void GenerateLocalUpdatableVersion(Platform platform)
        {
            //拷贝到本地的资源服务器
            string localServerFileDir = "../ClientBuild/Build_AssetBundle";
            string localServerFilePath = $"{localServerFileDir}/{platform}Version.txt";
            Directory.CreateDirectory(localServerFileDir);
            if (File.Exists(localServerFilePath))
            {
                File.Delete(localServerFilePath);
            }

            VersionInfoEditorData versionInfoEditorData = AssetDatabase.LoadAssetAtPath<VersionInfoEditorData>(VersionInfoEditorData.DataAssetPath);
            VersionInfoData versionInfoData = versionInfoEditorData.GetActiveVersionInfoData();

            VersionInfo versionInfo = versionInfoData.ToVersionInfo();
            versionInfo.UpdatePrefixUri = versionInfoData.GetCustomUpdatePrefixUri("http://192.168.1.18:80");
            if (!UriUtility.CheckUri(versionInfo.UpdatePrefixUri))
            {
                Debug.LogError($"{versionInfo.UpdatePrefixUri} is wrong, please check!");
            }

            File.WriteAllText(localServerFilePath, Newtonsoft.Json.JsonConvert.SerializeObject(versionInfo));
        }
    }
}