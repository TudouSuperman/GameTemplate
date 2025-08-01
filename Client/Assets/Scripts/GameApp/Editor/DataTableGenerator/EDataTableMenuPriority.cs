namespace GameApp.DataTable.Editor
{
    public enum EDataTableMenuPriority : short
    {
        TxtToBin = 0,
        HotTxtToBin,

        ExcelToBin = 20,
        ExcelToTxt,
        HotExcelToBin,
        HotExcelToTxt,

        ExcelToEnum = 40,
        HotExcelToEnum,

        HotExcelToLanguageXML = 60,
    }
}