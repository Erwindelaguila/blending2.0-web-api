using System.Text.Json.Nodes;
using ClosedXML.Excel;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Config;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Infrastructure.Config.Output;

namespace Function.Blending.Upload.Services.Write.Logistic.Sap;

public class ExcelWriteSapLogisticService
{
    public static void Execute(ExcelMappingOutputLogisticsConfig config , JsonObject dataContenedoresHomogenizacion, XLWorkbook workbook)
    {
        var hoja = workbook.Worksheet(config.Sap.SheetName);
        int startRow = config.Sap.StartRow;
        int starRowCabezeras = startRow + 1;
        var startColum = "B";
        
        foreach (var puerto in config.Homogenizacion.Puertos)
        {
            WritingCabeceraPuertoSap(hoja, puerto.Key, puerto.Value,starRowCabezeras, startColum );
            starRowCabezeras++;
        }
        
        WritingTotalesSap(hoja,starRowCabezeras,startColum);

        WritingDistribucionGruposSap(hoja, dataContenedoresHomogenizacion, config.Homogenizacion.Puertos, startRow, "B");

    }
    
    private static void WritingCabeceraPuertoSap(IXLWorksheet hoja, string key, Puerto puerto, int startRow, string startColum)
    {
        var cellRuma = hoja.Cell($"{startColum}{startRow}");
        cellRuma.Value = key;
        cellRuma.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92cddc"));
            
        var columnCant = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(startColum)+1);
        var cellCant = hoja.Cell($"{columnCant}{startRow}");
        cellCant.Value = ConfigHelper.TryToDouble(puerto.Soporte);
        cellCant.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92cddc"));
            
        var columnIdContenedor = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(startColum)+2);
        var cellIdContenedor = hoja.Cell($"{columnIdContenedor}{startRow}");
        cellIdContenedor.Value = puerto.CodeAfla;
        cellIdContenedor.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92cddc"));
    }
    
    private static void WritingTotalesSap(IXLWorksheet hoja, int startRow, string startColum)
    {
        var cellSinValue1 = hoja.Cell($"{startColum}{startRow}");
        cellSinValue1.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"));
        var columnSinValue = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(startColum)+1);
        var cellSinValue2 = hoja.Cell($"{columnSinValue}{startRow}");
        cellSinValue2.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"));
        var columnTotales = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(startColum)+2);
        var cellTotales = hoja.Cell($"{columnTotales}{startRow}");
        cellTotales.Value = "Totales";
        cellTotales.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"));
    }
    
        private static void WritingDistribucionGruposSap(
    IXLWorksheet hoja,
    JsonObject grupos,
    Dictionary<string, Puerto> puertos,
    int startRow,
    string startColum)
    {
        // Columna inicial en índice numérico
        int currentColIndex = ConfigHelper.ColumnLetterToNumber(startColum) + 3;
        int startDataRow = startRow + 1;

        foreach (var grupo in grupos)
        { 
            string nombreGrupo = grupo.Key;
            var distribucionObj = (JsonObject)grupo.Value["distribucion"];

            // Encabezado del grupo
            hoja.Cell(startRow, currentColIndex).Value = nombreGrupo;

            int row = startDataRow;

        // Escribir valores de cada puerto
            foreach (var puerto in puertos)
            {
                if (distribucionObj.TryGetPropertyValue(puerto.Key, out var valor) && valor != null)
                {
                    hoja.Cell(row, currentColIndex).Value = ConfigHelper.TryToDouble(valor);
                }
                row++;
            }

            int lastDataRow = row - 1; // última fila con datos
            int sumRow = row; // fila de la suma por columna

        hoja.Cell(sumRow, currentColIndex).FormulaA1 =
            $"=SUM({hoja.Cell(startDataRow, currentColIndex).Address}:{hoja.Cell(lastDataRow, currentColIndex).Address})";

        hoja.Cell(sumRow, currentColIndex)
            .Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"))
            .Font.SetBold();

        // Pasar a la siguiente columna
        currentColIndex++;
        }

        // === 1️⃣ Agregar columna de totales por fila ===
        hoja.Cell(startRow, currentColIndex).Value = "Totales";
        hoja.Cell(startRow, currentColIndex).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92d050")).Font.SetBold();

        for (int row = startDataRow; row < startDataRow + puertos.Count; row++)
        {
            string firstGroupCell = hoja.Cell(row, ConfigHelper.ColumnLetterToNumber(startColum) + 3).Address.ToString();
            string lastGroupCell = hoja.Cell(row, currentColIndex - 1).Address.ToString();

            hoja.Cell(row, currentColIndex).FormulaA1 = $"=SUM({firstGroupCell}:{lastGroupCell})";
            hoja.Cell(row, currentColIndex)
                .Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"))
                .Font.SetBold();
        }

        // === 2️⃣ Agregar total final de totales de fila ==
        int totalRow = startDataRow + puertos.Count; // fila después de los datos
        string firstTotalFilaCell = hoja.Cell(startDataRow, currentColIndex).Address.ToString();
        string lastTotalFilaCell = hoja.Cell(startDataRow + puertos.Count - 1, currentColIndex).Address.ToString();

        hoja.Cell(totalRow, currentColIndex).FormulaA1 = $"=SUM({firstTotalFilaCell}:{lastTotalFilaCell})";
        hoja.Cell(totalRow, currentColIndex)
            .Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"))
            .Font.SetBold();
    }
    
}