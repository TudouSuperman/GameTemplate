using System;
using System.Text;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace GameApp.DataTable.Editor
{
    public static class DataTableLocalizationGenerator
    {
        [MenuItem("GameApp/DataTable/Generate/Hotfix Excel To Language XML", priority = (short)EDataTableMenuPriority.HotExcelToLanguageXML)]
        public static void GenerateLocalizationFiles()
        {
            try
            {
                GenerateLocalizationFiles($"{DataTableConfig.GetDataTableConfig().HotfixExcelsFolder}/$Localization.xlsx",
                    "Assets/Res/Generate/TableData/Localization");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void GenerateLocalizationFiles(string inputPath, string outputPath)
        {
            Directory.CreateDirectory(outputPath);
            FileInfo excelFile = new FileInfo(inputPath);
            List<Tuple<string, string>> keyRemarks = new List<Tuple<string, string>>(); // 存储Key和策划备注

            using (ExcelPackage package = new ExcelPackage(excelFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                List<LanguageColumn> languages = new List<LanguageColumn>();
                int col = 3;

                while (col <= worksheet.Dimension.End.Column)
                {
                    string langName = worksheet.Cells[2, col].Text.Trim();
                    if (string.IsNullOrEmpty(langName)) break;
                    languages.Add(new LanguageColumn { Name = langName, ColumnIndex = col });
                    col++;
                }

                Dictionary<string, Dictionary<string, string>> langDicts = languages
                    .ToDictionary(lang => lang.Name, lang => new Dictionary<string, string>());

                int row = 5;
                while (row <= worksheet.Dimension.End.Row && !string.IsNullOrEmpty(worksheet.Cells[row, 1].Text))
                {
                    string key = worksheet.Cells[row, 1].Text.Trim();
                    string remark = worksheet.Cells[row, 2].Text.Trim(); // 获取策划备注
                    keyRemarks.Add(Tuple.Create(key, remark)); // 存储Key和备注

                    foreach (var lang in languages)
                    {
                        string text = worksheet.Cells[row, lang.ColumnIndex].Text.Trim();
                        if (!langDicts[lang.Name].ContainsKey(key))
                        {
                            langDicts[lang.Name].Add(key, text);
                        }
                    }

                    row++;
                }

                foreach (var lang in languages)
                {
                    string fileName = $"{lang.Name}.xml".Replace(" ", "_");
                    GenerateXmlFile(lang.Name, langDicts[lang.Name], Path.Combine(outputPath, fileName));
                    Debug.Log(Utility.Text.Format("Parse data table '{0}' success.", Path.Combine(outputPath, fileName)));
                }

                // 生成语言Key常量类
                GenerateLanguageKeyConstants(
                    keyRemarks,
                    "Assets/Scripts/GameApp/Hotfix/Code/Runtime/Generate/TableConst", // 输出路径
                    "LocalizationKey", // 类名
                    "GameApp.Hotfix" // 命名空间
                );
            }

            AssetDatabase.Refresh();
        }

        private static void GenerateXmlFile(string languageName, Dictionary<string, string> dict, string filePath)
        {
            XDocument xmlDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("Dictionaries",
                    new XElement("Dictionary",
                        new XAttribute("Language", languageName),
                        dict.Select(kv => new XElement("String",
                            new XAttribute("Key", kv.Key),
                            new XAttribute("Value", kv.Value)
                        ))
                    )
                )
            );

            using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                xmlDoc.Save(writer);
            }
        }

        private static void GenerateLanguageKeyConstants(
            List<Tuple<string, string>> keyRemarks,
            string outputPath,
            string className,
            string nameSpace)
        {
            StringBuilder sb = new StringBuilder();
            string filePath = Path.Combine(outputPath, $"{className}.cs");

            // 文件头部
            sb.AppendLine("// 此文件由工具自动生成，请勿直接修改。");
            sb.AppendLine($"// 生成时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("//------------------------------------------------------------\n");
            sb.AppendLine($"namespace {nameSpace}");
            sb.AppendLine("{");
            sb.AppendLine("    public static partial class HotConstant");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static class {className}");
            sb.AppendLine("        {");

            // 添加所有Key常量
            for (int i = 0; i < keyRemarks.Count; i++)
            {
                var (key, remark) = keyRemarks[i];
                string constantName = key.Replace('.', '_'); // 将点替换为下划线

                sb.AppendLine("            /// <summary>");
                sb.AppendLine($"            /// {remark}"); // 策划备注
                sb.AppendLine("            /// </summary>");
                sb.AppendLine($"            public const string {constantName} = \"{key}\";");

                if (i < keyRemarks.Count - 1) sb.AppendLine();
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            Debug.Log(Utility.Text.Format("Generate language keys constants '{0}' success.", filePath));
        }

        private class LanguageColumn
        {
            public string Name { get; set; }
            public int ColumnIndex { get; set; }
        }
    }
}