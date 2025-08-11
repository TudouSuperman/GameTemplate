using UnityEditor;

namespace GameApp.Editor
{
    public static class DataTableGeneratorAll
    {
        [MenuItem("GameApp/DataTable/Generate/Gen All By Bin", false, (short)EDataTableMenuPriority.GenAllByBin)]
        public static void GenAllByBin()
        {
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Config/ResetDataTableConfig");
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Generate/Excel To Bin");
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Generate/Hotfix Excel To Bin");
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Generate/Excel To Enum");
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Generate/Hotfix Excel To Enum");
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Generate/Hotfix Excel To Language XML");
            AssetDatabase.Refresh();
        }

        [MenuItem("GameApp/DataTable/Generate/Gen All By Txt", false, (short)EDataTableMenuPriority.GenAllByTxt)]
        public static void GenAllByTxt()
        {
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Config/ResetDataTableConfig");
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Generate/Excel To Txt");
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Generate/Hotfix Excel To Txt");
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Generate/Excel To Enum");
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Generate/Hotfix Excel To Enum");
            EditorApplication.ExecuteMenuItem("GameApp/DataTable/Generate/Hotfix Excel To Language XML");
            AssetDatabase.Refresh();
        }
    }
}