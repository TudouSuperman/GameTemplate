using System.IO;
using System.Text;
using System;
using UnityEditor;
using UnityEngine;
using GameFramework;
using OfficeOpenXml;

namespace GameApp.DataTable.Editor
{
    public static class DataTableEnumGenerator
    {
        [MenuItem("GameApp/DataTable/Generate/Excel To Enum", false, (short)EDataTableMenuPriority.ExcelToEnum)]
        private static void GenerateDataTableEnum()
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

                            GenerateEnumFile(dataTableProcessor, sheet.Name);
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("GameApp/DataTable/Generate/Hot Excel To Enum", false, (short)EDataTableMenuPriority.HotExcelToEnum)]
        private static void GenerateHotDataTableEnum()
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

                            GenerateEnumFile(dataTableProcessor, sheet.Name);
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        public static void GenerateEnumFile(DataTableProcessor dataTableProcessor, string dataTableName)
        {
            dataTableProcessor.SetCodeTemplate(DataTableConfig.GetDataTableConfig().CSharpEnumCodeTemplateFileName, Encoding.UTF8);
            dataTableProcessor.SetCodeGenerator(DataTableCodeGenerator);

            string csharpCodeFileName = Utility.Path.GetRegularPath(Path.Combine(DataTableConfig.GetDataTableConfig().HotEnumCodePath, $"E{dataTableName}ID.cs"));
            if (!dataTableProcessor.GenerateCodeFile(csharpCodeFileName, Encoding.UTF8, dataTableName) && File.Exists(csharpCodeFileName))
            {
                File.Delete(csharpCodeFileName);
            }
        }

        private static void DataTableCodeGenerator(DataTableProcessor dataTableProcessor, StringBuilder codeContent, object userData)
        {
            string dataTableName = (string)userData;

            codeContent.Replace("__DATA_TABLE_CREATE_TIME__", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            codeContent.Replace("__DATA_TABLE_NAME_SPACE__", "GameApp.Hot");
            codeContent.Replace("__DATA_TABLE_ENUM_NAME__", $"E{dataTableName}ID");
            //codeContent.Replace("__DATA_TABLE_COMMENT__", dataTableProcessor.GetValue(0, 1) + "。");
            codeContent.Replace("__DATA_TABLE_ENUM_ITEM__", GenerateEnumItems(dataTableProcessor));
        }

        private static string GenerateEnumItems(DataTableProcessor dataTableProcessor)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool firstProperty = true;

            int startRow = DataTableConfig.GetDataTableConfig().ContentStartRow;

            stringBuilder
                .AppendLine("        /// <summary>")
                .AppendFormat("        /// {0}", "无").AppendLine()
                .AppendLine("        /// </summary>")
                .AppendFormat("        {0} = {1},", "None", "0").AppendLine().AppendLine();

            for (int i = startRow; i < dataTableProcessor.RawRowCount; i++)
            {
                int index = i - startRow;

                if (firstProperty)
                {
                    firstProperty = false;
                }
                else
                {
                    stringBuilder.AppendLine().AppendLine();
                }

                stringBuilder
                    .AppendLine("        /// <summary>")
                    .AppendFormat("        /// {0}", dataTableProcessor.GetValue(i, 2)).AppendLine()
                    .AppendLine("        /// </summary>")
                    .AppendFormat("        {0} = {1},", dataTableProcessor.GetValue(i, 3), dataTableProcessor.GetValue(i, 1));
            }

            return stringBuilder.ToString();
        }
    }
}