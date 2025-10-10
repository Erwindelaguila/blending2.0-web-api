using ClosedXML.Excel;
using Function.Blending.Upload.Infrastructure.Config;

namespace Function.Blending.Upload.Services.Write.Sap;

public class ExcelWriteSapService
{
    public static void Execute( ExcelMappingOutputSapConfig config,
        List<Dictionary<string,string>> dataGetSap, XLWorkbook workbook)
    {
        var hoja = workbook.Worksheet(config.Data.SheetName);
        
        int startRow = config.Data.StartRow;
        int starRowValues = startRow+1;

        WriteCamps(config.Data.Fijos, dataGetSap,hoja, startRow, starRowValues );
        WriteCamps(config.Data.ParametrosCalidad, dataGetSap,hoja, startRow, starRowValues );
        WriteCamps(config.Data.OtrosParametros, dataGetSap,hoja, startRow, starRowValues );
    }

    private static void WriteCamps( Dictionary<string,CampoExcel> paramentro, List<Dictionary<string,string>> dataGetSap,  IXLWorksheet hoja, 
        int startRow, int starRowValues)
    {
        foreach (var itemValueParam in paramentro)
        {
            var celdaHeader = hoja.Cell($"{itemValueParam.Value.Column}{startRow}");
            celdaHeader.Value = itemValueParam.Value.Header;
            // Aplicar formato: negrita + borde fino
            celdaHeader.Style.Font.Bold = true;
            celdaHeader.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            celdaHeader.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            
            
            var key = itemValueParam.Key;
            var starRowItem = starRowValues;
            foreach (var itemSap in dataGetSap)
            {
                var value = itemSap[$"{key}"];
                hoja.Cell($"{itemValueParam.Value.Column}{starRowItem}").Value = value;
                starRowItem++;
            }
            hoja.Column(itemValueParam.Value.Column).AdjustToContents();
        }
        
    }
}