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
    public sealed class DataTableConfig : ScriptableObject
    {
        /// <summary>
        /// 数据表存放文件夹路径
        /// </summary>
        public string DataTableFolderPath;

        /// <summary>
        /// Hot 数据表存放文件夹路径
        /// </summary>
        public string HotDataTableFolderPath;

        /// <summary>
        /// Excel 存放的文件夹路径
        /// </summary>
        public string ExcelsFolder;

        /// <summary>
        /// Hot Excel 存放的文件夹路径
        /// </summary>
        public string HotExcelsFolder;

        /// <summary>
        /// 数据表 C# 实体类生成文件夹路径
        /// </summary>
        public string CSharpCodePath;

        /// <summary>
        /// Hot 数据表 C# 实体类生成文件夹路径
        /// </summary>
        public string HotCSharpCodePath;

        /// <summary>
        /// 数据表 C# 实体类模板存放路径
        /// </summary>
        public string CSharpCodeTemplateFileName;

        /// <summary>
        /// 数据表命名空间
        /// </summary>
        public string NameSpace;

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
            if (Directory.Exists(HotDataTableFolderPath))
            {
                var txtFolder = new DirectoryInfo(HotDataTableFolderPath);
                TxtFilePaths = txtFolder.GetFiles("*.txt", SearchOption.TopDirectoryOnly)
                    .Select(_ => Utility.Path.GetRegularPath(_.FullName))
                    .ToArray();
                DataTableNames = txtFolder.GetFiles("*.txt", SearchOption.TopDirectoryOnly)
                    .Select(file => Path.GetFileNameWithoutExtension(file.Name))
                    .ToArray();
            }

            if (Directory.Exists(HotExcelsFolder))
            {
                var excelFolder = new DirectoryInfo(HotExcelsFolder);
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
            codeGeneratorSettingConfig.DataTableFolderPath = "Assets/Res/Generate/TableData/NoHot";
            codeGeneratorSettingConfig.HotDataTableFolderPath = "Assets/Res/Generate/TableData/Hot";
            codeGeneratorSettingConfig.ExcelsFolder = $"{Application.dataPath}/../../ClientExcel/NoHot";
            codeGeneratorSettingConfig.HotExcelsFolder = $"{Application.dataPath}/../../ClientExcel/Hot";
            codeGeneratorSettingConfig.CSharpCodePath = "Assets/Scripts/GameApp/Generate/TableCode";
            codeGeneratorSettingConfig.HotCSharpCodePath = "Assets/Scripts/GameApp/HotUpdate/Code/Runtime/Generate/TableCode";
            codeGeneratorSettingConfig.CSharpCodeTemplateFileName = "Assets/Res/Editor/Config/DataTableCodeTemplate.txt";
            codeGeneratorSettingConfig.NameSpace = "GameApp.DataTable";

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

            s_DataTableConfig.DataTableFolderPath = "Assets/Res/Generate/NoHot/TableData";
            s_DataTableConfig.HotDataTableFolderPath = "Assets/Res/Generate/Hot/TableData";
            s_DataTableConfig.ExcelsFolder = $"{Application.dataPath}/../../ClientExcel/NoHot";
            s_DataTableConfig.HotExcelsFolder = $"{Application.dataPath}/../../ClientExcel/Hot";
            s_DataTableConfig.CSharpCodePath = "Assets/Scripts/GameApp/Generate/TableCode";
            s_DataTableConfig.HotCSharpCodePath = "Assets/Scripts/GameApp/HotUpdate/Code/Runtime/Generate/TableCode";
            s_DataTableConfig.CSharpCodeTemplateFileName = "Assets/Res/Editor/Config/DataTableCodeTemplate.txt";
            s_DataTableConfig.NameSpace = "GameApp.DataTable";

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