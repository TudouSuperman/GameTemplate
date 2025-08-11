//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameApp.Editor
{
    public sealed class DataTableGenerator
    {
        private static readonly Regex EndWithNumberRegex = new Regex(@"\d+$");
        private static readonly Regex NameRegex = new Regex(@"^[A-Z][A-Za-z0-9_]*$");

        private static DataTableConfig s_DataTableConfig;

        public static DataTableProcessor CreateDataTableProcessor(string dataTableName)
        {
            return new DataTableProcessor(Utility.Path.GetRegularPath(Path.Combine(DataTableConfig.GetDataTableConfig().DataTableFolderPath, dataTableName + ".txt")), Encoding.UTF8, 1, 2, null, 3, 4, 1);
        }

        public static DataTableProcessor CreateExcelDataTableProcessor(OfficeOpenXml.ExcelWorksheet sheet)
        {
            return new DataTableProcessor(sheet, 1, 2, null, 3, 4, 1);
        }

        public static bool CheckRawData(DataTableProcessor dataTableProcessor, string dataTableName)
        {
            for (int i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                string name = dataTableProcessor.GetName(i);
                if (string.IsNullOrEmpty(name) || name == "#")
                {
                    continue;
                }

                if (!NameRegex.IsMatch(name))
                {
                    Debug.LogWarning(Utility.Text.Format("Check raw data failure. DataTableName='{0}' Name='{1}'", dataTableName, name));
                    return false;
                }
            }

            return true;
        }

        public static void GenerateDataFile(DataTableProcessor dataTableProcessor, string dataTableName, string folderPath)
        {
            // 确保输出目录存在
            Directory.CreateDirectory(folderPath);
            string binaryDataFileName = Utility.Path.GetRegularPath(Path.Combine(folderPath, dataTableName + ".bytes"));
            if (!dataTableProcessor.GenerateDataFile(binaryDataFileName) && File.Exists(binaryDataFileName))
            {
                File.Delete(binaryDataFileName);
            }
        }

        public static void GenerateCodeFile(DataTableProcessor dataTableProcessor, string dataTableName, string folderPath)
        {
            // 确保输出目录存在
            Directory.CreateDirectory(folderPath);
            dataTableProcessor.SetCodeTemplate(DataTableConfig.GetDataTableConfig().CSharpCodeTemplateFileName, Encoding.UTF8);
            dataTableProcessor.SetCodeGenerator(DataTableCodeGenerator);
            string csharpCodeFileName = Utility.Path.GetRegularPath(Path.Combine(folderPath, "DR" + dataTableName + ".cs"));
            if (!dataTableProcessor.GenerateCodeFile(csharpCodeFileName, Encoding.UTF8, dataTableName) && File.Exists(csharpCodeFileName))
            {
                File.Delete(csharpCodeFileName);
            }
        }

        private static void DataTableCodeGenerator(DataTableProcessor dataTableProcessor, StringBuilder codeContent, object userData)
        {
            string dataTableName = (string)userData;

            codeContent.Replace("__DATA_TABLE_CREATE_TIME__", DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
            codeContent.Replace("__DATA_TABLE_NAME_SPACE__", DataTableConfig.GetDataTableConfig().NameSpace);
            codeContent.Replace("__DATA_TABLE_CLASS_NAME__", "DR" + dataTableName);
            codeContent.Replace("__DATA_TABLE_COMMENT__", dataTableProcessor.GetValue(0, 1) + "。");
            codeContent.Replace("__DATA_TABLE_ID_COMMENT__", "获取" + dataTableProcessor.GetComment(dataTableProcessor.IdColumn) + "。");
            codeContent.Replace("__DATA_TABLE_PROPERTIES__", GenerateDataTableProperties(dataTableProcessor));
            codeContent.Replace("__DATA_TABLE_PARSER__", GenerateDataTableParser(dataTableProcessor));
        }

        public static void GenerateHotfixCodeFile(DataTableProcessor dataTableProcessor, string dataTableName, string folderPath)
        {
            // 确保输出目录存在
            Directory.CreateDirectory(folderPath);
            dataTableProcessor.SetCodeTemplate(DataTableConfig.GetDataTableConfig().CSharpCodeTemplateFileName, Encoding.UTF8);
            dataTableProcessor.SetCodeGenerator(DataTableHotfixCodeGenerator);
            string csharpCodeFileName = Utility.Path.GetRegularPath(Path.Combine(folderPath, "DR" + dataTableName + ".cs"));
            if (!dataTableProcessor.GenerateCodeFile(csharpCodeFileName, Encoding.UTF8, dataTableName) && File.Exists(csharpCodeFileName))
            {
                File.Delete(csharpCodeFileName);
            }
        }

        private static void DataTableHotfixCodeGenerator(DataTableProcessor dataTableProcessor, StringBuilder codeContent, object userData)
        {
            string dataTableName = (string)userData;

            codeContent.Replace("__DATA_TABLE_CREATE_TIME__", DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
            codeContent.Replace("__DATA_TABLE_NAME_SPACE__", DataTableConfig.GetDataTableConfig().HotfixNameSpace);
            codeContent.Replace("__DATA_TABLE_CLASS_NAME__", "DR" + dataTableName);
            codeContent.Replace("__DATA_TABLE_COMMENT__", dataTableProcessor.GetValue(0, 1) + "。");
            codeContent.Replace("__DATA_TABLE_ID_COMMENT__", "获取" + dataTableProcessor.GetComment(dataTableProcessor.IdColumn) + "。");
            codeContent.Replace("__DATA_TABLE_PROPERTIES__", GenerateDataTableProperties(dataTableProcessor));
            codeContent.Replace("__DATA_TABLE_PARSER__", GenerateDataTableParser(dataTableProcessor));
        }

        private static string GenerateDataTableProperties(DataTableProcessor dataTableProcessor)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool firstProperty = true;
            for (int i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                if (dataTableProcessor.IsCommentColumn(i))
                {
                    // 注释列
                    continue;
                }

                if (dataTableProcessor.IsIdColumn(i))
                {
                    // 编号列
                    continue;
                }

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
                    .AppendFormat("        /// 获取{0}。", dataTableProcessor.GetComment(i)).AppendLine()
                    .AppendLine("        /// </summary>")
                    .AppendFormat("        public {0} {1}", dataTableProcessor.GetLanguageKeyword(i), dataTableProcessor.GetName(i)).AppendLine()
                    .AppendLine("        {")
                    .AppendLine("            get;")
                    .AppendLine("            private set;")
                    .Append("        }");
            }

            return stringBuilder.ToString();
        }

        private static string GenerateDataTableParser(DataTableProcessor dataTableProcessor)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder
                .AppendLine("        public override bool ParseDataRow(string dataRowString, object userData)")
                .AppendLine("        {")
                .AppendLine("            string[] columnStrings = dataRowString.Split(GameApp.DataTableExtension.DataSplitSeparators);")
                .AppendLine("            for (int i = 0; i < columnStrings.Length; i++)")
                .AppendLine("            {")
                .AppendLine("                columnStrings[i] = columnStrings[i].Trim(GameApp.DataTableExtension.DataTrimSeparators);")
                .AppendLine("            }")
                .AppendLine()
                .AppendLine("            int index = 0;");

            for (int i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                if (dataTableProcessor.IsCommentColumn(i))
                {
                    // 注释列
                    stringBuilder.AppendLine("            index++;");
                    continue;
                }

                if (dataTableProcessor.IsIdColumn(i))
                {
                    // 编号列
                    stringBuilder.AppendLine("            m_Id = int.Parse(columnStrings[index++]);");
                    continue;
                }

                if (dataTableProcessor.IsSystem(i))
                {
                    string languageKeyword = dataTableProcessor.GetLanguageKeyword(i);
                    if (languageKeyword == "string")
                    {
                        stringBuilder.AppendFormat("            {0} = columnStrings[index++];", dataTableProcessor.GetName(i)).AppendLine();
                    }
                    else
                    {
                        stringBuilder.AppendFormat("            {0} = {1}.Parse(columnStrings[index++]);", dataTableProcessor.GetName(i), languageKeyword).AppendLine();
                    }
                }
                else
                {
                    if (dataTableProcessor.IsListColumn(i))
                    {
                        var t = dataTableProcessor.GetDataProcessor(i).GetType().GetGenericArguments();
                        var dataProcessor = Activator.CreateInstance(t[0]) as DataTableProcessor.DataProcessor;
                        string typeName = dataProcessor.Type.Name;
                        stringBuilder
                            .AppendFormat("\t\t\t{0} = GameApp.Hotfix.DataTableExtension.Parse{1}List(columnStrings[index++]);",
                                dataTableProcessor.GetName(i), typeName).AppendLine();
                        continue;
                    }

                    if (dataTableProcessor.IsArrayColumn(i))
                    {
                        var t = dataTableProcessor.GetDataProcessor(i).GetType().GetGenericArguments();
                        var dataProcessor =
                            Activator.CreateInstance(t[0]) as DataTableProcessor.DataProcessor;
                        string typeName = dataProcessor.Type.Name;
                        stringBuilder
                            .AppendFormat("\t\t\t{0} = GameApp.Hotfix.DataTableExtension.Parse{1}Array(columnStrings[index++]);",
                                dataTableProcessor.GetName(i), typeName).AppendLine();
                        continue;
                    }

                    if (dataTableProcessor.IsDictionaryColumn(i))
                    {
                        var t = dataTableProcessor.GetDataProcessor(i).GetType().GetGenericArguments();
                        var dataProcessorT1 =
                            Activator.CreateInstance(t[0]) as DataTableProcessor.DataProcessor;
                        var dataProcessorT2 =
                            Activator.CreateInstance(t[1]) as DataTableProcessor.DataProcessor;
                        var dataProcessorT1TypeName = dataProcessorT1.Type.Name;
                        var dataProcessorT2TypeName = dataProcessorT2.Type.Name;
                        stringBuilder.AppendFormat(
                                "\t\t\t{0} = GameApp.Hotfix.DataTableExtension.Parse{1}{2}Dictionary(columnStrings[index++]);",
                                dataTableProcessor.GetName(i), dataProcessorT1TypeName, dataProcessorT2TypeName)
                            .AppendLine();
                        continue;
                    }

                    stringBuilder.AppendFormat("\t\t\t{0} = GameApp.DataTableExtension.Parse{1}(columnStrings[index++]);", dataTableProcessor.GetName(i), dataTableProcessor.GetType(i).Name).AppendLine();
                }
            }

            stringBuilder.AppendLine()
                .AppendLine("            return true;")
                .AppendLine("        }")
                .AppendLine()
                .AppendLine("        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)")
                .AppendLine("        {")
                .AppendLine("            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))")
                .AppendLine("            {")
                .AppendLine("                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))")
                .AppendLine("                {");

            for (var i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                if (dataTableProcessor.IsCommentColumn(i))
                    // 注释列
                    continue;

                if (dataTableProcessor.IsIdColumn(i))
                {
                    // 编号列
                    stringBuilder.AppendLine("                    m_Id = binaryReader.Read7BitEncodedInt32();");
                    continue;
                }

                if (dataTableProcessor.IsIdColumn(i))
                {
                    // 编号列
                    stringBuilder.AppendLine("                m_Id = binaryReader.ReadInt32();");
                    continue;
                }

                var languageKeyword = dataTableProcessor.GetLanguageKeyword(i);
                if (dataTableProcessor.IsListColumn(i))
                {
                    var t = dataTableProcessor.GetDataProcessor(i).GetType().GetGenericArguments();
                    var dataProcessor =
                        Activator.CreateInstance(t[0]) as DataTableProcessor.DataProcessor;
                    string typeName = dataProcessor.Type.Name;
                    stringBuilder.AppendFormat("\t\t\t\t\t{0} = binaryReader.Read{1}List();",
                        dataTableProcessor.GetName(i), typeName).AppendLine();
                    continue;
                }

                if (dataTableProcessor.IsArrayColumn(i))
                {
                    var t = dataTableProcessor.GetDataProcessor(i).GetType().GetGenericArguments();
                    var dataProcessor = Activator.CreateInstance(t[0]) as DataTableProcessor.DataProcessor;
                    string typeName = dataProcessor.Type.Name;
                    stringBuilder.AppendFormat("\t\t\t\t\t{0} = binaryReader.Read{1}Array();",
                        dataTableProcessor.GetName(i), typeName).AppendLine();
                    continue;
                }

                if (dataTableProcessor.IsDictionaryColumn(i))
                {
                    var t = dataTableProcessor.GetDataProcessor(i).GetType().GetGenericArguments();
                    var dataProcessorT1 =
                        Activator.CreateInstance(t[0]) as DataTableProcessor.DataProcessor;
                    var dataProcessorT2 =
                        Activator.CreateInstance(t[1]) as DataTableProcessor.DataProcessor;
                    var dataProcessorT1TypeName = dataProcessorT1.Type.Name;
                    var dataProcessorT2TypeName = dataProcessorT2.Type.Name;
                    stringBuilder.AppendFormat("\t\t\t\t\t{0} = binaryReader.Read{1}{2}Dictionary();",
                            dataTableProcessor.GetName(i), dataProcessorT1TypeName, dataProcessorT2TypeName)
                        .AppendLine();
                    continue;
                }

                if (languageKeyword == "int" || languageKeyword == "uint" || languageKeyword == "long" ||
                    languageKeyword == "ulong")
                    stringBuilder.AppendFormat("                    {0} = binaryReader.Read7BitEncoded{1}();",
                        dataTableProcessor.GetName(i), dataTableProcessor.GetType(i).Name).AppendLine();
                else
                    stringBuilder.AppendFormat("                    {0} = binaryReader.Read{1}();",
                        dataTableProcessor.GetName(i), dataTableProcessor.GetType(i).Name).AppendLine();
            }

            stringBuilder
                .AppendLine("                }")
                .AppendLine("            }")
                .AppendLine()
                .AppendLine("            return true;")
                .Append("        }");

            return stringBuilder.ToString();
        }
    }
}