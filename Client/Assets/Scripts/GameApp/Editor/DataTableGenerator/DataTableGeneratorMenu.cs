//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.IO;
using GameApp.Procedure;
using GameFramework;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

namespace GameApp.DataTable.Editor
{
    public sealed class DataTableGeneratorMenu
    {
        [MenuItem("GameApp/DataTable/Generate/Txt To Bin", priority = 1)]
        private static void GenerateDataTables()
        {
            foreach (string dataTableName in ProcedurePreload.DataTableNames)
            {
                DataTableProcessor dataTableProcessor = DataTableGenerator.CreateDataTableProcessor(dataTableName);
                if (!DataTableGenerator.CheckRawData(dataTableProcessor, dataTableName))
                {
                    Debug.LogError(Utility.Text.Format("Check raw data failure. DataTableName='{0}'", dataTableName));
                    break;
                }

                DataTableGenerator.GenerateDataFile(dataTableProcessor, dataTableName);
                DataTableGenerator.GenerateCodeFile(dataTableProcessor, dataTableName);
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("GameApp/DataTable/Generate/Excel To Bin", priority = 2)]
        public static void GenerateDataTablesFormExcelNotFileSystem()
        {
            DataTableConfig.GetDataTableConfig().RefreshDataTables();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            foreach (var excelFile in DataTableConfig.GetDataTableConfig().ExcelFilePaths)
            {
                using (FileStream fileStream = new FileStream(excelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (ExcelPackage excelPackage = new ExcelPackage(fileStream))
                    {
                        for (int i = 0; i < excelPackage.Workbook.Worksheets.Count; i++)
                        {
                            ExcelWorksheet sheet = excelPackage.Workbook.Worksheets[i];
                            var dataTableProcessor = DataTableGenerator.CreateExcelDataTableProcessor(sheet);
                            if (!DataTableGenerator.CheckRawData(dataTableProcessor, sheet.Name))
                            {
                                Debug.LogError(Utility.Text.Format("Check raw data failure. DataTableName='{0}'",
                                    sheet.Name));
                                break;
                            }

                            DataTableGenerator.GenerateDataFile(dataTableProcessor, sheet.Name);
                            DataTableGenerator.GenerateCodeFile(dataTableProcessor, sheet.Name);
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("GameApp/DataTable/Generate/Excel To Txt", priority = 10)]
        public static void ExcelToTxt()
        {
            DataTableConfig.GetDataTableConfig().RefreshDataTables();
            if (!Directory.Exists(DataTableConfig.GetDataTableConfig().ExcelsFolder))
            {
                Debug.LogError($"{DataTableConfig.GetDataTableConfig().ExcelsFolder} is not exist!");
                return;
            }

            ExcelExtension.ExcelToTxt(DataTableConfig.GetDataTableConfig().ExcelsFolder, DataTableConfig.GetDataTableConfig().DataTableFolderPath);
            AssetDatabase.Refresh();
        }

        [MenuItem("GameApp/DataTable/Config/CreateDataTableConfig", priority = 1)]
        public static void CreateDataTableConfig()
        {
            DataTableConfig.CreateDataTableConfig();
        }

        [MenuItem("GameApp/DataTable/Config/ResetDataTableConfig", priority = 2)]
        public static void ResetDataTableConfig()
        {
            DataTableConfig.ResetDataTableConfig();
        }
    }
}