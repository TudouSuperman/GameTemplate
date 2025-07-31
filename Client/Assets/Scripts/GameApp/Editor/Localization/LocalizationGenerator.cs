using System;
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
    public static class LocalizationGenerator
    {
        [MenuItem("GameApp/DataTable/Generate/Excel To Language", priority = 40)]
        public static void GenerateLocalizationFiles()
        {
            try
            {
                GenerateLocalizationFiles($"{DataTableConfig.GetDataTableConfig().HotExcelsFolder}/$Localization.xlsx", "Assets/Res/Generate/GameHot/Localization");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void GenerateLocalizationFiles(string inputPath, string outputPath)
        {
            // 确保输出目录存在
            Directory.CreateDirectory(outputPath);
            // 加载 Excel 文件
            FileInfo excelFile = new FileInfo(inputPath);
            using (ExcelPackage package = new ExcelPackage(excelFile))
            {
                // 获取第一个工作表
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                // 动态获取语言列表（从第2行获取列头）
                List<LanguageColumn> languages = new List<LanguageColumn>();
                int col = 3; // 从第3列开始（C列）
                // 遍历所有语言列（直到遇到空列头）
                while (col <= worksheet.Dimension.End.Column)
                {
                    string langName = worksheet.Cells[2, col].Text.Trim();
                    if (string.IsNullOrEmpty(langName)) break;
                    languages.Add(new LanguageColumn
                    {
                        Name = langName,
                        ColumnIndex = col
                    });
                    col++;
                }

                // 为每种语言创建字典
                Dictionary<string, Dictionary<string, string>> langDicts = new Dictionary<string, Dictionary<string, string>>();
                foreach (var lang in languages)
                {
                    langDicts[lang.Name] = new Dictionary<string, string>();
                }

                // 从第5行开始读取有效数据（跳过标题行）
                int row = 5;
                while (row <= worksheet.Dimension.End.Row && !string.IsNullOrEmpty(worksheet.Cells[row, 1].Text))
                {
                    string key = worksheet.Cells[row, 1].Text.Trim();
                    // 为每种语言添加条目
                    foreach (var lang in languages)
                    {
                        string text = worksheet.Cells[row, lang.ColumnIndex].Text.Trim();
                        // 即使为空也添加（如Dialog.Other）
                        if (!langDicts[lang.Name].ContainsKey(key))
                        {
                            langDicts[lang.Name].Add(key, text);
                        }
                    }

                    row++;
                }

                // 为每种语言生成XML文件
                foreach (var lang in languages)
                {
                    string fileName = $"{lang.Name}.xml".Replace(" ", "_"); // 替换空格
                    GenerateXmlFile(lang.Name, langDicts[lang.Name], Path.Combine(outputPath, fileName));
                    Debug.Log(Utility.Text.Format("Parse data table '{0}' success.", Path.Combine(outputPath, fileName)));
                }
            }
        }

        private static void GenerateXmlFile(string languageName, Dictionary<string, string> dict, string filePath)
        {
            // 创建XML文档结构
            XDocument xmlDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("Dictionaries",
                    new XElement("Dictionary",
                        new XAttribute("Language", languageName),
                        // 为每个键值对创建String元素
                        dict.Select(kv =>
                            new XElement("String",
                                new XAttribute("Key", kv.Key),
                                new XAttribute("Value", kv.Value)
                            )
                        )
                    )
                )
            );
            // 保存文件（使用UTF-8编码）
            using (var writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                xmlDoc.Save(writer);
            }
        }

        private class LanguageColumn
        {
            public string Name { get; set; }
            public int ColumnIndex { get; set; }
        }
    }
}