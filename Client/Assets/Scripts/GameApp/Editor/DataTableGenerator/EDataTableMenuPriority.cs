namespace GameApp.Editor
{
    public enum EDataTableMenuPriority : short
    {
        ExcelToBin = 20,
        ExcelToTxt,
        HotExcelToBin,
        HotExcelToTxt,

        ExcelToEnum = 40,
        HotExcelToEnum,

        HotExcelToLanguageXML = 60,

        GenAllByBin = short.MaxValue - 1,
        GenAllByTxt = short.MaxValue,
    }
}