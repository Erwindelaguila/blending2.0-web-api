using System.Text.Json.Nodes;
using ClosedXML.Excel;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Infrastructure.Config;


namespace Function.Blending.Upload.Services;

public class ExcelWriterService
{
    public static void Write<T>(
        IXLWorksheet hoja,
        JsonObject data,
        Dictionary<string, ExcelConfig> parametros,
        int startRow,
        bool usarItemKeyEnColumnaA = false 
    )
    {
        foreach (var param in parametros)
        {
            var key = param.Key;
            var config = param.Value;

            hoja.Cell($"{config.Column}{startRow}").Value = config.Header;
            var row = startRow + 1;

            foreach (var entry in data)
            {
                var itemKey = entry.Key;
                var node = entry.Value[$"{key}"];
                
                object? valorFinal = null;
                string? error = null;

                var expectedType = (config.DataType ?? "").ToLower();

                try
                {
                    switch (expectedType)
                    {
                        case "string":
                            valorFinal = ((JsonValue?)node)?.GetValue<string>();
                            break;

                        case "number":
                            // Intentar convertir a double (puede adaptarse a decimal/int si prefieres)
                            valorFinal = ((JsonValue?)node)?.GetValue<double>();
                            break;

                        default:
                            valorFinal = node?.ToString(); // fallback
                            break;
                    }
                }
                catch
                {
                    error = $"Valor \"{node}\" no es tipo {expectedType}";
                }

                object? valorEscrito = config.Column == "A"
                    ? (usarItemKeyEnColumnaA ? itemKey : valorFinal)
                    : valorFinal;

                hoja.Cell($"{config.Column}{row}").Value = error ?? valorEscrito?.ToString();
                row++;
            }
        }
    }

    public static void WriteSapLogistic(ExcelMappingOutputLogisticsConfig config , JsonObject dataContenedoresHomogenizacion, XLWorkbook workbook)
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

    public static void WriteHomogenizacionContenedores( ExcelMappingOutputLogisticsConfig config , JsonObject dataContenedoresHomogenizacion, XLWorkbook workbook )
    {
        var hoja = workbook.Worksheet(config.Homogenizacion.SheetName);
        int startRow = config.Homogenizacion.StartRow;
        
        //Ultima Columna dado que son dinamicos
        string ultimaColumna = "";
        
        foreach (var puerto in config.Homogenizacion.Puertos)
        {
            WritingCabeceraPuerto(hoja, puerto.Key, puerto.Value, startRow);
            ultimaColumna = puerto.Value.Column;
        }
        
        WritingTotalesFinales(hoja, ultimaColumna, startRow);
        
        WritingDistribucionGrupos(hoja, dataContenedoresHomogenizacion, config.Homogenizacion.Puertos, startRow + 4); // fila 14 si startRow=10
        
        int filaTotalesPorColumna = startRow + 4 + dataContenedoresHomogenizacion.Count;
        WritingTotalesPorPuerto(hoja, filaTotalesPorColumna, config.Homogenizacion.Puertos, dataContenedoresHomogenizacion.Count);
        
        
        //Esta columna es opcional
        var colOptionalPromedSumaPro = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(ultimaColumna) + 2);
        hoja.Cell($"{colOptionalPromedSumaPro}{startRow}").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92cddc"));
        hoja.Cell($"{colOptionalPromedSumaPro}{startRow + 1}").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92cddc"));
        
        
        var startColumnParametrosCalidad = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(ultimaColumna) + 3);
        foreach (var parametroCalidad in config.Homogenizacion.PametrosCalidad)
        {
            WritingCabeceraParamentrosCalidad(hoja, parametroCalidad.Value , startColumnParametrosCalidad , startRow);
            startColumnParametrosCalidad = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(startColumnParametrosCalidad) + 1);
        }

        WritingValuesParamentrosCalidad(startRow, ultimaColumna, hoja, config.Homogenizacion.PametrosCalidad,
            dataContenedoresHomogenizacion);
    }
    
    

    private static void WritingValuesParamentrosCalidad(int startRow , string ultimaColumna, IXLWorksheet hoja , Dictionary<string,ParametroCalidad> parametrosCalidad
    , JsonObject dataContenedoresHomogenizacion)
    {
        
        
        var starRowCalidades = startRow + 4;
        var filaPromedios = starRowCalidades - 1;
        
        var colcalidadSinImpacto = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(ultimaColumna) + 2);
        
        var colCalidadInicial =  ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(ultimaColumna) + 3);
        
        var valueSinImpacto = ConfigHelper.TryToDouble("0.00");
        
        var cellSinImpacto = hoja.Cell($"{colcalidadSinImpacto}{filaPromedios}");
        cellSinImpacto.Value = valueSinImpacto;
        cellSinImpacto.Style.NumberFormat.Format = "0.00";
        hoja.Cell($"{colcalidadSinImpacto}{filaPromedios}").Value = 0.00;
        hoja.Cell($"{colcalidadSinImpacto}{filaPromedios}").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"));
        
        var acumuladores = new Dictionary<string, List<double>>();
            
        foreach (var parametro in parametrosCalidad)
        {
            acumuladores[parametro.Key] = new List<double>();
        }

        var cont = 0;
        foreach (var grupo in dataContenedoresHomogenizacion)
        {
            JsonObject composicionObj = (JsonObject)grupo.Value["composicion"];
            int currentRowv1 = starRowCalidades + cont;
            int copyCurrentRowv1 = starRowCalidades + cont;
            
            var cell = hoja.Cell($"{colcalidadSinImpacto}{copyCurrentRowv1}");
            cell.Value = valueSinImpacto;
            cell.Style.NumberFormat.Format = "0.00"; // fuerza dos decimales
            
            var colCalidad = colCalidadInicial;
            foreach (var parametro in parametrosCalidad)
            { 
                var calidadKey = parametro.Key;
                
                if (composicionObj.TryGetPropertyValue(calidadKey, out var valor))
                {
                        var valorDouble = ConfigHelper.TryToDouble(valor);
                        hoja.Cell($"{colCalidad}{currentRowv1}").Value = valorDouble;
                        acumuladores[calidadKey].Add(valorDouble);
                }
                
                colCalidad = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(colCalidad) + 1);
            }
            copyCurrentRowv1++;
            cont++;
        }
        
        int ultimaFila = starRowCalidades + cont; // Última fila usada
        int filaExtra = ultimaFila;           // Fila siguiente

        // Pintar columna sin impacto
        var cellExtraSinImpacto = hoja.Cell($"{colcalidadSinImpacto}{filaExtra}");
        cellExtraSinImpacto.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"));

        // Pintar columnas de parámetros
        var colCalidadExtra = colCalidadInicial;
        foreach (var parametro in parametrosCalidad)
        {
            var cellExtra = hoja.Cell($"{colCalidadExtra}{filaExtra}");
            cellExtra.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"));

            colCalidadExtra = ConfigHelper.ColumnNumberToLetter(
                ConfigHelper.ColumnLetterToNumber(colCalidadExtra) + 1);
        }
        
        
        
        var colCalPromedios = colCalidadInicial;
        foreach (var parametro in parametrosCalidad)
        {
            // Rango: por ejemplo, G8:G10
            var celdaInicio = $"{colCalPromedios}{starRowCalidades}";
            var celdaFin = $"{colCalPromedios}{starRowCalidades + cont - 1}";

            // Fórmula de promedio
            var celdaPromedio = hoja.Cell($"{colCalPromedios}{filaPromedios}");
            celdaPromedio.FormulaA1 = $"AVERAGE({celdaInicio}:{celdaFin})";
            celdaPromedio.Style.NumberFormat.Format = "0.00"; // Dos decimales

            // Opcional: Negrita
            hoja.Cell($"{colCalPromedios}{filaPromedios}").Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"));

            colCalPromedios = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(colCalPromedios) + 1);
        }
    }
    
    private static void WritingCabeceraPuerto(IXLWorksheet hoja, string key, Puerto puerto, int startRow)
    {
        string col = puerto.Column;

        hoja.Cell($"{col}{startRow}").Value = puerto.Nombre;
        hoja.Cell($"{col}{startRow}").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92cddc")).Font.SetBold();

        hoja.Cell($"{col}{startRow + 1}").Value = key;
        hoja.Cell($"{col}{startRow + 1}").Style
            .Alignment.SetTextRotation(45)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Font.SetBold();
        hoja.Cell($"{col}{startRow + 1}").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92cddc"));

        hoja.Cell($"{col}{startRow + 2}").Value = puerto.Soporte.ToString();
        hoja.Cell($"{col}{startRow + 2}").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92cddc")).Font.SetBold();

        hoja.Cell($"{col}{startRow + 3}").Value = puerto.CodeAfla;
        hoja.Cell($"{col}{startRow + 3}").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92cddc")).Font.SetBold();
    }
    
    
    private static void WritingTotalesFinales(IXLWorksheet hoja, string ultimaColumna, int startRow)
    {
        var siguienteCol = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(ultimaColumna) + 1);

        for (int i = 0; i <= 3; i++)
        {
            int fila = startRow + i;
            var celda = hoja.Cell($"{siguienteCol}{fila}");

            celda.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"));
            celda.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            celda.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            if (i == 3)
            {
                celda.Value = "Totales";
                celda.Style.Font.SetBold();
            }
        }
    }
    
    private static void WritingDistribucionGrupos(IXLWorksheet hoja,  JsonObject grupos, Dictionary<string, Puerto> puertos, int startRow)
    {
        int filaOffset = 0;
        var columnasOrdenadas = puertos.Values
            .Select(p => p.Column)
            .OrderBy(ConfigHelper.ColumnLetterToNumber)
            .ToList();

        foreach (var grupo in grupos)
        {
            string nombreGrupo = grupo.Key;
            JsonObject distribucionObj = (JsonObject)grupo.Value["distribucion"];
    

            int currentRow = startRow + filaOffset;
            
            hoja.Cell($"A{currentRow}").Value = nombreGrupo;

            foreach (var puerto in puertos)
            {
                string puertoKey = puerto.Key;
                string col = puerto.Value.Column;
                
                if (distribucionObj.TryGetPropertyValue(puertoKey, out var valor) && valor != null)
                {
                    hoja.Cell($"{col}{currentRow}").Value =ConfigHelper.TryToDouble(valor);
                }
            }

            string colInicio = columnasOrdenadas.First();
            string colFin = columnasOrdenadas.Last();
            string celdaSuma = $"{ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(colFin) + 1)}{currentRow}";
            hoja.Cell(celdaSuma).FormulaA1 = $"=SUM({colInicio}{currentRow}:{colFin}{currentRow})";
            hoja.Cell(celdaSuma).Style
                .Fill.SetBackgroundColor(XLColor.FromHtml("#92d050")) 
                .Font.SetBold();

            filaOffset++;
        }
        
    }
    
    private static void WritingTotalesPorPuerto(IXLWorksheet hoja, int filaTotal, Dictionary<string, Puerto> puertos, int totalContenedores)
    {
        // Ordenamos columnas por orden alfabético (para fórmulas)
        var columnasOrdenadas = puertos.Values
            .Select(p => p.Column)
            .OrderBy(ConfigHelper.ColumnLetterToNumber)
            .ToList();

        foreach (var puerto in puertos)
        {
            string col = puerto.Value.Column;
            string celdaInicio = $"{col}{filaTotal - totalContenedores}";
            string celdaFin = $"{col}{filaTotal - 1}";

            hoja.Cell($"{col}{filaTotal}").FormulaA1 = $"=SUM({celdaInicio}:{celdaFin})";
            hoja.Cell($"{col}{filaTotal}").Style
                .Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"))
                .Font.SetBold();
        }

        // Celda "TOTALES" en la columna A
        hoja.Cell($"A{filaTotal}").Value = "TOTALES";
        hoja.Cell($"A{filaTotal}").Style
            .Fill.SetBackgroundColor(XLColor.FromHtml("#92d050"))
            .Font.SetBold();

        // 👉 Nueva celda de suma horizontal al final de los totales verticales
        string colInicio = columnasOrdenadas.First();
        string colFin = columnasOrdenadas.Last();
        string nuevaCol = ConfigHelper.ColumnNumberToLetter(ConfigHelper.ColumnLetterToNumber(colFin) + 1);
        string celdaTotalFinal = $"{nuevaCol}{filaTotal}";

        hoja.Cell(celdaTotalFinal).FormulaA1 = $"=SUM({colInicio}{filaTotal}:{colFin}{filaTotal})";
        hoja.Cell(celdaTotalFinal).Style
            .Fill.SetBackgroundColor(XLColor.FromHtml("#92d050")) // Celeste fuerte
            .Font.SetBold();
    }
    
    private static void WritingCabeceraParamentrosCalidad(IXLWorksheet hoja, ParametroCalidad calidad, string col, int startRow)
    {
        hoja.Cell($"{col}{startRow}").Value = calidad.Header;
        hoja.Cell($"{col}{startRow}").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92cddc")).Font.SetBold();
        hoja.Cell($"{col}{startRow + 1}").Value = calidad.Name;
        hoja.Cell($"{col}{startRow + 1}").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#92cddc")).Font.SetBold();
        hoja.Cell($"{col}{startRow + 2}").Value = ConfigHelper.TryToDouble(calidad.Cantidad);
        hoja.Cell($"{col}{startRow + 2}").Style.Font.SetBold();
    }


    public static void WriteExcelSap( ExcelSapMappingOutputConfig config,
        List<Dictionary<string,string>> dataGetSap, XLWorkbook workbook)
    {
        var hoja = workbook.Worksheet(config.Data.SheetName);
        
        int startRow = config.Data.StartRow;
        int starRowValues = startRow+1;

        WriteCamps(config.Data.Fijos, dataGetSap,hoja, startRow, starRowValues );
        WriteCamps(config.Data.ParametrosCalidad, dataGetSap,hoja, startRow, starRowValues );
        WriteCamps(config.Data.OtrosParametros, dataGetSap,hoja, startRow, starRowValues );
    }

    public static void WriteCamps( Dictionary<string,CampoExcel> paramentro, List<Dictionary<string,string>> dataGetSap,  IXLWorksheet hoja, 
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