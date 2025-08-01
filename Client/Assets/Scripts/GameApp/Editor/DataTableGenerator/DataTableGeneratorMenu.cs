//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.IO;
using GameFramework;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

namespace GameApp.DataTable.Editor
{
    public sealed class DataTableGeneratorMenu
    {
        [MenuItem("GameApp/DataTable/Generate/Txt To Bin", priority = (short)EDataTableMenuPriority.TxtToBin)]
        private static void GenerateDataTables()
        {
            foreach (string dataTableName in Procedure.ProcedurePreload.DataTableNames)
            {
                DataTableProcessor dataTableProcessor = DataTableGenerator.CreateDataTableProcessor(dataTableName);
                if (!DataTableGenerator.CheckRawData(dataTableProcessor, dataTableName))
                {
                    Debug.LogError(Utility.Text.Format("Check raw data failure. DataTableName='{0}'", dataTableName));
                    break;
                }

                DataTableGenerator.GenerateDataFile(dataTableProcessor, dataTableName, DataTableConfig.GetDataTableConfig().DataTableFolderPath);
                DataTableGenerator.GenerateCodeFile(dataTableProcessor, dataTableName, DataTableConfig.GetDataTableConfig().CSharpCodePath);
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("GameApp/DataTable/Generate/Excel To Bin", priority = (short)EDataTableMenuPriority.ExcelToBin)]
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
                                Debug.LogError(Utility.Text.Format("Check raw data failure. DataTableName='{0}'", sheet.Name));
                                break;
                            }

                            DataTableGenerator.GenerateDataFile(dataTableProcessor, sheet.Name, DataTableConfig.GetDataTableConfig().DataTableFolderPath);
                            DataTableGenerator.GenerateCodeFile(dataTableProcessor, sheet.Name, DataTableConfig.GetDataTableConfig().CSharpCodePath);
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("GameApp/DataTable/Generate/Excel To Txt", priority = (short)EDataTableMenuPriority.ExcelToTxt)]
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

        [MenuItem("GameApp/DataTable/Generate/Hot Txt To Bin", priority = (short)EDataTableMenuPriority.HotTxtToBin)]
        private static void HotGenerateDataTables()
        {
            foreach ((string dataTableName, Type dataRowType) in Hot.Procedure.ProcedurePreload.DataTableNames)
            {
                DataTableProcessor dataTableProcessor = DataTableGenerator.CreateDataTableProcessor(dataTableName);
                if (!DataTableGenerator.CheckRawData(dataTableProcessor, dataTableName))
                {
                    Debug.LogError(Utility.Text.Format("Check raw data failure. DataTableName='{0}'", dataTableName));
                    break;
                }

                DataTableGenerator.GenerateDataFile(dataTableProcessor, dataTableName, DataTableConfig.GetDataTableConfig().HotDataTableFolderPath);
                DataTableGenerator.GenerateCodeFile(dataTableProcessor, dataTableName, DataTableConfig.GetDataTableConfig().HotCSharpCodePath);
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("GameApp/DataTable/Generate/Hot Excel To Bin", priority = (short)EDataTableMenuPriority.HotExcelToBin)]
        public static void HotGenerateDataTablesFormExcelNotFileSystem()
        {
            DataTableConfig.GetDataTableConfig().RefreshHotDataTables();
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
                                Debug.LogError(Utility.Text.Format("Check raw data failure. DataTableName='{0}'", sheet.Name));
                                break;
                            }

                            DataTableGenerator.GenerateDataFile(dataTableProcessor, sheet.Name, DataTableConfig.GetDataTableConfig().HotDataTableFolderPath);
                            DataTableGenerator.GenerateCodeFile(dataTableProcessor, sheet.Name, DataTableConfig.GetDataTableConfig().HotCSharpCodePath);
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("GameApp/DataTable/Generate/Hot Excel To Txt", priority = (short)EDataTableMenuPriority.HotExcelToTxt)]
        public static void HotExcelToTxt()
        {
            DataTableConfig.GetDataTableConfig().RefreshHotDataTables();
            if (!Directory.Exists(DataTableConfig.GetDataTableConfig().HotExcelsFolder))
            {
                Debug.LogError($"{DataTableConfig.GetDataTableConfig().HotExcelsFolder} is not exist!");
                return;
            }

            ExcelExtension.ExcelToTxt(DataTableConfig.GetDataTableConfig().HotExcelsFolder, DataTableConfig.GetDataTableConfig().HotDataTableFolderPath);
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