#if UNITY_HOTFIX
using GameApp.Editor;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Extension.Editor;

namespace GameApp.Hotfix.Editor
{
    public static class PreprocessCompileDll
    {
        [UGFBuildOnPreprocessPlatform(1)]
        public static void CompileDll(Platform platform)
        {
            BuildAssemblyTool.Build(BuildAssemblyHelper.GetBuildTarget(platform));
        }
    }
}
#endif