using ClosedXML.Excel;

namespace Function.Blending.Upload.Helpers;

public class ExcelXMLHelper
{
    public static XLWorkbook? GetWorkbook(string directory, string configFileName)
    {
        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), directory);
        var excelPath = Path.Combine(rootPath, configFileName);
        return new XLWorkbook(excelPath);
    }
}