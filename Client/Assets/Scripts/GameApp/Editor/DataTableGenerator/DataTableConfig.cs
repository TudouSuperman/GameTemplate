using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using GameFramework;

namespace GameApp.DataTable.Editor
{
    /// <summary>
    /// 数据表编辑器配置相关数据
    /// </summary>
    [CreateAssetMenu(fileName = "DataTableConfig", menuName = "GameApp/DataTableConfig")]
    public sealed class DataTableConfig : ScriptableObject
    {
        /// <summary>
        /// 数据表存放文件夹路径
        /// </summary>
        public string DataTableFolderPath;

        /// <summary>
        /// Hotfix 数据表存放文件夹路径
        /// </summary>
        public string HotfixDataTableFolderPath;

        /// <summary>
        /// Excel 存放的文件夹路径
        /// </summary>
        public string ExcelsFolder;

        /// <summary>
        /// Hotfix Excel 存放的文件夹路径
        /// </summary>
        public string HotfixExcelsFolder;

        /// <summary>
        /// 数据表 C# 实体类生成文件夹路径
        /// </summary>
        public string CSharpCodePath;

        /// <summary>
        /// Hotfix 数据表 C# 实体类生成文件夹路径
        /// </summary>
        public string HotfixCSharpCodePath;

        /// <summary>
        /// 数据表 C# 实体类模板存放路径
        /// </summary>
        public string CSharpCodeTemplateFileName;

        /// <summary>
        /// 数据表 枚举 实体类模板存放路径
        /// </summary>
        public string CSharpEnumCodeTemplateFileName;

        /// <summary>
        /// 数据表 常量 实体类模板存放路径
        /// </summary>
        public string CSharpConstCodeTemplateFileName;

        /// <summary>
        /// Hotfix 数据表 枚举 实体类生成文件夹路径
        /// </summary>
        public string HotfixEnumCodePath;

        /// <summary>
        /// 数据表命名空间
        /// </summary>
        public string NameSpace;

        /// <summary>
        /// Hotfix 数据表命名空间
        /// </summary>
        public string HotfixNameSpace;

        /// <summary>
        /// 数据表扩展类文件夹路径
        /// </summary>
        public string ExtensionDirectoryPath;

        /// <summary>
        /// 数据表文件路径
        /// </summary>
        [NonSerialized]
        public string[] TxtFilePaths;

        /// <summary>
        /// 数据表文件名
        /// </summary>
        [NonSerialized]
        public string[] DataTableNames;

        /// <summary>
        /// Excel表文件路径
        /// </summary>
        [NonSerialized]
        public string[] ExcelFilePaths;

        //所有行列 是逻辑行列从 0 开始，但是 EPPlus 需要从 1 开始遍历 使用时需要 +1
        /// <summary>
        /// 字段名所在行
        /// </summary>
        public int NameRow;

        /// <summary>
        /// 类型名所在行
        /// </summary>
        public int TypeRow;

        /// <summary>
        /// 注释所在行
        /// </summary>
        public int CommentRow;

        /// <summary>
        /// 内容开始行
        /// </summary>
        public int ContentStartRow;

        /// <summary>
        /// id 所在列
        /// </summary>
        public int IdColumn;

        public void RefreshDataTables()
        {
            if (Directory.Exists(DataTableFolderPath))
            {
                var txtFolder = new DirectoryInfo(DataTableFolderPath);
                TxtFilePaths = txtFolder.GetFiles("*.txt", SearchOption.TopDirectoryOnly)
                    .Select(_ => Utility.Path.GetRegularPath(_.FullName))
                    .ToArray();
                DataTableNames = txtFolder.GetFiles("*.txt", SearchOption.TopDirectoryOnly)
                    .Select(file => Path.GetFileNameWithoutExtension(file.Name))
                    .ToArray();
            }

            if (Directory.Exists(ExcelsFolder))
            {
                var excelFolder = new DirectoryInfo(ExcelsFolder);
                ExcelFilePaths = excelFolder.GetFiles("*.xlsx", SearchOption.AllDirectories)
                    .Where(_ => !_.Name.StartsWith("~$") && !_.Name.StartsWith("$")).Select(_ => Utility.Path.GetRegularPath(_.FullName))
                    .ToArray();
            }
        }

        public void RefreshHotDataTables()
        {
            if (Directory.Exists(HotfixDataTableFolderPath))
            {
                var txtFolder = new DirectoryInfo(HotfixDataTableFolderPath);
                TxtFilePaths = txtFolder.GetFiles("*.txt", SearchOption.TopDirectoryOnly)
                    .Select(_ => Utility.Path.GetRegularPath(_.FullName))
                    .ToArray();
                DataTableNames = txtFolder.GetFiles("*.txt", SearchOption.TopDirectoryOnly)
                    .Select(file => Path.GetFileNameWithoutExtension(file.Name))
                    .ToArray();
            }

            if (Directory.Exists(HotfixExcelsFolder))
            {
                var excelFolder = new DirectoryInfo(HotfixExcelsFolder);
                ExcelFilePaths = excelFolder.GetFiles("*.xlsx", SearchOption.AllDirectories)
                    .Where(_ => !_.Name.StartsWith("~$") && !_.Name.StartsWith("$")).Select(_ => Utility.Path.GetRegularPath(_.FullName))
                    .ToArray();
            }
        }

        private static string s_DataTableConfigPath = "Assets/Res/Editor/Config/DataTableConfig.asset";

        private static DataTableConfig s_DataTableConfig;

        public static DataTableConfig GetDataTableConfig()
        {
            if (s_DataTableConfig == null)
            {
                DataTableConfig dataTableConfig = AssetDatabase.LoadAssetAtPath<DataTableConfig>(s_DataTableConfigPath);
                if (dataTableConfig == null)
                {
                    throw new Exception($"不存在{nameof(DataTableConfig)} 请调用 DataTable/CreateDataTableConfig 创建配置");
                }

                s_DataTableConfig = dataTableConfig;
            }

            return s_DataTableConfig;
        }


        public static void CreateDataTableConfig()
        {
            try
            {
                GetDataTableConfig();
            }
            catch
            {
                // ignored
            }

            if (s_DataTableConfig != null)
            {
                EditorUtility.DisplayDialog("警告", $"已存在{nameof(DataTableConfig)}，路径:{s_DataTableConfig}", "确认");
                return;
            }

            DataTableConfig codeGeneratorSettingConfig = CreateInstance<DataTableConfig>();
            codeGeneratorSettingConfig.DataTableFolderPath = "Assets/Res/Generate/TableData/Game";
            codeGeneratorSettingConfig.HotfixDataTableFolderPath = "Assets/Res/Generate/TableData/GameHotfix";
            codeGeneratorSettingConfig.ExcelsFolder = $"{Application.dataPath}/../../ClientExcel/Game";
            codeGeneratorSettingConfig.HotfixExcelsFolder = $"{Application.dataPath}/../../ClientExcel/GameHotfix";
            codeGeneratorSettingConfig.CSharpCodePath = "Assets/Scripts/GameApp/Generate/TableCode";
            codeGeneratorSettingConfig.HotfixCSharpCodePath = "Assets/Scripts/GameApp/Hotfix/Code/Runtime/Generate/TableCode";
            codeGeneratorSettingConfig.CSharpCodeTemplateFileName = "Assets/Res/Editor/Config/DataTableCodeTemplate.txt";
            codeGeneratorSettingConfig.CSharpEnumCodeTemplateFileName = "Assets/Res/Editor/Config/DataTableEnumCodeTemplate.txt";
            codeGeneratorSettingConfig.CSharpConstCodeTemplateFileName = "Assets/Res/Editor/Config/DataTableConstCodeTemplate.txt";
            codeGeneratorSettingConfig.HotfixEnumCodePath = "Assets/Scripts/GameApp/Hotfix/Code/Runtime/Generate/TableEnum";
            codeGeneratorSettingConfig.NameSpace = "GameApp.DataTable";
            codeGeneratorSettingConfig.HotfixNameSpace = "GameApp.Hotfix.DataTable";
            codeGeneratorSettingConfig.ExtensionDirectoryPath = "Assets/Scripts/GameApp/DataTable";

            codeGeneratorSettingConfig.NameRow = 1;
            codeGeneratorSettingConfig.TypeRow = 2;
            codeGeneratorSettingConfig.CommentRow = 3;
            codeGeneratorSettingConfig.ContentStartRow = 4;
            codeGeneratorSettingConfig.IdColumn = 1;
            AssetDatabase.CreateAsset(codeGeneratorSettingConfig, s_DataTableConfigPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void ResetDataTableConfig()
        {
            try
            {
                GetDataTableConfig();
            }
            catch
            {
                // ignored
            }

            if (s_DataTableConfig == null)
            {
                EditorUtility.DisplayDialog("警告", $"不存在{nameof(DataTableConfig)}，路径:{s_DataTableConfig}，请先创建！", "确认");
                return;
            }

            s_DataTableConfig.DataTableFolderPath = "Assets/Res/Generate/Game/TableData";
            s_DataTableConfig.HotfixDataTableFolderPath = "Assets/Res/Generate/GameHotfix/TableData";
            s_DataTableConfig.ExcelsFolder = $"{Application.dataPath}/../../ClientExcel/Game";
            s_DataTableConfig.HotfixExcelsFolder = $"{Application.dataPath}/../../ClientExcel/GameHotfix";
            s_DataTableConfig.CSharpCodePath = "Assets/Scripts/GameApp/Generate/TableCode";
            s_DataTableConfig.HotfixCSharpCodePath = "Assets/Scripts/GameApp/Hotfix/Code/Runtime/Generate/TableCode";
            s_DataTableConfig.CSharpCodeTemplateFileName = "Assets/Res/Editor/Config/DataTableCodeTemplate.txt";
            s_DataTableConfig.CSharpEnumCodeTemplateFileName = "Assets/Res/Editor/Config/DataTableEnumCodeTemplate.txt";
            s_DataTableConfig.CSharpConstCodeTemplateFileName = "Assets/Res/Editor/Config/DataTableConstCodeTemplate.txt";
            s_DataTableConfig.HotfixEnumCodePath = "Assets/Scripts/GameApp/Hotfix/Code/Runtime/Generate/TableEnum";
            s_DataTableConfig.NameSpace = "GameApp.DataTable";
            s_DataTableConfig.HotfixNameSpace = "GameApp.Hotfix.DataTable";
            s_DataTableConfig.ExtensionDirectoryPath = "Assets/Scripts/GameApp/DataTable";

            s_DataTableConfig.NameRow = 1;
            s_DataTableConfig.TypeRow = 2;
            s_DataTableConfig.CommentRow = 3;
            s_DataTableConfig.ContentStartRow = 4;
            s_DataTableConfig.IdColumn = 1;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}