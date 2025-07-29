using UnityEngine;

[CreateAssetMenu(fileName = "LanguageExportConfig", menuName = "Localization/Language Export Config")]
public sealed class LanguageExportConfig : ScriptableObject
{
    public enum SourceType
    {
        TextArea,
        TextAsset
    }

    [Header("数据源设置")]
    public SourceType sourceType = SourceType.TextAsset;

    [TextArea(10, 30)] 
    [Tooltip("直接在文本区域粘贴表格数据")]
    public string inputTable;
    
    [Tooltip("引用包含表格数据的文本文件")]
    public TextAsset tableTextAsset;
    
    [Header("导出设置")]
    public string exportPath = "Assets/Res/Generate/Localization/";
    
    [Tooltip("语言列名（从表格第二行获取）")]
    public string[] languageColumns;
}