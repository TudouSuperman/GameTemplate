using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public class LanguageExporterWindow : EditorWindow
{
    private LanguageExportConfig config;
    private Vector2 scrollPosition;
    private string lastAssetPath;
    private TextAsset lastTextAsset;

    [MenuItem("GameApp/Language Tool Editor")]
    public static void ShowWindow()
    {
        GetWindow<LanguageExporterWindow>("Language Tool Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("语言导出配置", EditorStyles.boldLabel);

        // 选择配置文件
        config = (LanguageExportConfig)EditorGUILayout.ObjectField("配置文件", config, typeof(LanguageExportConfig), false);

        if (config == null)
        {
            EditorGUILayout.HelpBox("请先创建或选择一个LanguageExportConfig配置文件", MessageType.Info);
            if (GUILayout.Button("创建新配置"))
            {
                CreateNewConfig();
            }

            return;
        }

        // 显示配置
        EditorGUILayout.Space();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("数据源设置", EditorStyles.boldLabel);
        config.sourceType = (LanguageExportConfig.SourceType)EditorGUILayout.EnumPopup("数据源类型", config.sourceType);

        switch (config.sourceType)
        {
            case LanguageExportConfig.SourceType.TextArea:
                EditorGUILayout.LabelField("表格数据", EditorStyles.boldLabel);
                config.inputTable = EditorGUILayout.TextArea(config.inputTable, GUILayout.Height(200));
                break;

            case LanguageExportConfig.SourceType.TextAsset:
                config.tableTextAsset = (TextAsset)EditorGUILayout.ObjectField("表格文件", config.tableTextAsset, typeof(TextAsset), false);
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("导出路径", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        config.exportPath = EditorGUILayout.TextField(config.exportPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择导出目录", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                // 转换为相对路径
                if (path.StartsWith(Application.dataPath))
                {
                    config.exportPath = "Assets" + path.Substring(Application.dataPath.Length);
                }
                else
                {
                    config.exportPath = path;
                }
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("检测到的语言列", EditorStyles.boldLabel);
        if (config.languageColumns != null && config.languageColumns.Length > 0)
        {
            for (int i = 0; i < config.languageColumns.Length; i++)
            {
                EditorGUILayout.LabelField($"列 {i + 1}: {config.languageColumns[i]}");
            }
        }
        else
        {
            EditorGUILayout.HelpBox("未检测到语言列，请先解析表格", MessageType.Warning);
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(20);

        // 操作按钮
        if (GUILayout.Button("解析表格语言列", GUILayout.Height(30)))
        {
            ParseLanguageColumns();
        }

        if (GUILayout.Button("导出所有语言XML", GUILayout.Height(40)))
        {
            ExportAllLanguages();
        }
    }

    private void CreateNewConfig()
    {
        config = CreateInstance<LanguageExportConfig>();
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/LanguageExportConfig.asset");
        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = config;
    }

    private string GetTableContent()
    {
        string content = "";
        switch (config.sourceType)
        {
            case LanguageExportConfig.SourceType.TextArea:
                content = config.inputTable;
                break;

            case LanguageExportConfig.SourceType.TextAsset:
                content = config.tableTextAsset != null ? config.tableTextAsset.text : "";
                break;
        }
        
        // 统一替换所有回车符
        return content.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    private void ParseLanguageColumns()
    {
        string tableContent = GetTableContent();

        if (string.IsNullOrEmpty(tableContent))
        {
            Debug.LogError("表格数据为空");
            return;
        }

        string[] lines = tableContent.Split('\n');

        // 查找语言行（以"#\tKey\t"开头的行）
        string languageLine = lines.FirstOrDefault(line => line.StartsWith("#\tKey\t"));

        if (string.IsNullOrEmpty(languageLine))
        {
            Debug.LogError("未找到语言行，请确保表格包含以'#\\tKey\\t'开头的行");
            return;
        }

        // 分割语言行并清理
        string[] columns = languageLine.Split('\t')
            .Select(col => CleanCellValue(col))
            .Where(col => !string.IsNullOrWhiteSpace(col))
            .Skip(1) // 跳过开头的 #
            .ToArray();

        if (columns.Length < 2)
        {
            Debug.LogError("未检测到有效的语言列");
            return;
        }

        // 第一列是"Key"，之后的是语言列
        config.languageColumns = columns.Skip(1).ToArray();
        EditorUtility.SetDirty(config);
        Debug.Log($"检测到 {config.languageColumns.Length} 种语言: {string.Join(", ", config.languageColumns)}");
    }

    private void ExportAllLanguages()
    {
        if (config.languageColumns == null || config.languageColumns.Length == 0)
        {
            Debug.LogError("请先解析语言列");
            return;
        }

        // 确保导出目录存在
        if (!Directory.Exists(config.exportPath))
        {
            Directory.CreateDirectory(config.exportPath);
        }

        // 获取表格内容
        string tableContent = GetTableContent();
        if (string.IsNullOrEmpty(tableContent))
        {
            Debug.LogError("表格数据为空");
            return;
        }

        // 解析表格数据
        var languageData = ParseInputTable(tableContent, config.languageColumns);

        // 为每种语言生成XML
        foreach (var language in config.languageColumns)
        {
            GenerateXMLForLanguage(language, languageData[language]);
        }

        AssetDatabase.Refresh();
        Debug.Log($"成功导出 {config.languageColumns.Length} 种语言到: {config.exportPath}");
    }

    private Dictionary<string, Dictionary<string, string>> ParseInputTable(string tableContent, string[] languages)
    {
        var languageData = new Dictionary<string, Dictionary<string, string>>();

        // 初始化语言字典
        foreach (var lang in languages)
        {
            languageData[lang] = new Dictionary<string, string>();
        }

        string[] lines = tableContent.Split('\n');
        int dataStartIndex = -1;

        // 查找数据开始行（跳过所有注释行）
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
            {
                dataStartIndex = i;
                break;
            }
        }

        if (dataStartIndex == -1)
        {
            Debug.LogError("未找到有效数据行");
            return languageData;
        }

        // 处理数据行
        for (int i = dataStartIndex; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // 分割制表符并清理每列
            string[] columns = line.Split('\t')
                .Select(col => CleanCellValue(col))
                .Where(col => !string.IsNullOrWhiteSpace(col))
                .ToArray();

            // 确保有足够的列（Key + 至少一种语言）
            if (columns.Length < 1) continue;

            string key = columns[0];

            // 为每种语言添加翻译
            for (int langIndex = 0; langIndex < languages.Length; langIndex++)
            {
                string lang = languages[langIndex];

                // 如果该语言列存在，则使用该值，否则为空字符串
                string value = (langIndex + 1 < columns.Length) ? columns[langIndex + 1] : "";

                languageData[lang][key] = value;
            }
        }

        return languageData;
    }
    
    // 清理单元格值（移除回车/换行和特殊标记）
    private string CleanCellValue(string value)
    {
        // 移除回车/换行和首尾空格
        string cleaned = value
            .Replace("\r", "")
            .Replace("\n", "")
            .Trim();
        
        // 处理特殊标记的空值
        return (cleaned == "-" || cleaned == "null") ? "" : cleaned;
    }

    private void GenerateXMLForLanguage(string language, Dictionary<string, string> translations)
    {
        // 清理语言名称（移除特殊字符）
        string cleanLanguage = CleanLanguageName(language);
        
        XDocument xDoc = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement("Dictionaries",
                new XElement("Dictionary",
                    new XAttribute("Language", cleanLanguage),
                    CreateStringElements(translations)
                )
            )
        );

        // 生成安全文件名
        string safeFileName = GetSafeFileName(cleanLanguage);
        string filePath = Path.Combine(config.exportPath, $"{safeFileName}.xml");

        // 确保目录存在
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        xDoc.Save(filePath);
        Debug.Log($"已创建: {filePath}");
    }

    // 清理语言名称（移除非法XML字符）
    private string CleanLanguageName(string language)
    {
        // 移除控制字符和特殊符号
        return Regex.Replace(language, @"[\x00-\x1F]", "");
    }

    // 生成安全文件名
    private string GetSafeFileName(string language)
    {
        // 获取操作系统不允许的文件名字符
        char[] invalidChars = Path.GetInvalidFileNameChars();

        // 替换所有非法字符为下划线
        string safeName = string.Join("_", language.Split(invalidChars));
        
        // 移除连续的下划线
        safeName = Regex.Replace(safeName, @"_+", "_");
        
        // 如果文件名以点开头，添加前缀
        if (safeName.StartsWith("."))
        {
            safeName = "lang_" + safeName;
        }
        
        // 确保文件名不为空
        if (string.IsNullOrWhiteSpace(safeName))
        {
            safeName = "unknown_language";
        }
        
        return safeName.Trim('_');
    }

    private IEnumerable<XElement> CreateStringElements(Dictionary<string, string> translations)
    {
        foreach (var entry in translations)
        {
            yield return new XElement("String",
                new XAttribute("Key", entry.Key),
                new XAttribute("Value", entry.Value ?? "")
            );
        }
    }
}