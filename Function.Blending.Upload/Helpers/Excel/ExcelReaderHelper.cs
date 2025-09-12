using ClosedXML.Excel;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace Function.Blending.Upload.Helpers;

public static class ExcelReaderHelper
{
    public static ExcelMappingConfig LoadYamlConfig(string yamlPath)
    {
        var yamlContent = File.ReadAllText(yamlPath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<ExcelMappingConfig>(yamlContent);
    }

    public static List<ParsedRowDto> ParseXlsmFromBytes(byte[] fileBytes, ExcelMappingConfig config)
    {
        using var ms = new MemoryStream(fileBytes);
        using var workbook = new XLWorkbook(ms);
        var worksheet = workbook.Worksheet(config.SheetName);
        var result = new List<ParsedRowDto>();

        for (int row = config.StartRow; ; row++)
        {
            var cellB = worksheet.Cell(row, "B");
            if (cellB.IsEmpty())
                break; // asumimos fin de datos si columna B (rumaNro) está vacía

            var dto = new ParsedRowDto
            {
                Fijos = new Dictionary<string, string>(),
                ParametrosCalidad = new Dictionary<string, string>(),
                OtrosValores = new Dictionary<string, string>()
            };

            foreach (var prop in config.Fijos.GetType().GetProperties())
            {
                var colLetter = prop.GetValue(config.Fijos)?.ToString();
                if (!string.IsNullOrEmpty(colLetter))
                {
                    var value = worksheet.Cell(row, colLetter).GetValue<string>().Trim();
                    dto.Fijos[prop.Name] = value;
                }
            }

            foreach (var kv in config.ParametrosCalidad)
            {
                var value = worksheet.Cell(row, kv.Value).GetValue<string>().Trim();
                dto.ParametrosCalidad[kv.Key] = value;
            }

            foreach (var kv in config.OtrosValores)
            {
                var value = worksheet.Cell(row, kv.Value).GetValue<string>().Trim();
                dto.OtrosValores[kv.Key] = value;
            }

            result.Add(dto);
        }

        return result;
    }

    public static Task<List<ParsedRowDto>> LeerFilasDesdeExcelAsync(byte[] fileBytes, ExcelMappingConfig config)
    {
        var result = new List<ParsedRowDto>();

        using var stream = new MemoryStream(fileBytes);
        using var workbook = new XLWorkbook(stream);

        // Validación segura de la existencia de la hoja
        if (!workbook.Worksheets.Any(ws => ws.Name.Equals(config.SheetName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"No se encontró la hoja '{config.SheetName}' en el archivo Excel.");
        }
        var worksheet = workbook.Worksheet(config.SheetName);

        var row = config.StartRow;
        while (!worksheet.Row(row).IsEmpty())
        {
            var rowDto = new ParsedRowDto();

            // Parte fija (fijos)
            foreach (var prop in config.Fijos.GetType().GetProperties())
            {
                var colLetter = prop.GetValue(config.Fijos)?.ToString();
                if (!string.IsNullOrEmpty(colLetter))
                {
                    var cell = worksheet.Cell($"{colLetter}{row}");
                    rowDto.Fijos[prop.Name] = cell.GetString();
                }
            }

            // Parte dinámica: parámetros de calidad
            foreach (var kvp in config.ParametrosCalidad)
            {
                var cell = worksheet.Cell($"{kvp.Value}{row}");
                rowDto.ParametrosCalidad[kvp.Key] = cell.GetString();
            }

            // Parte dinámica: otros valores
            foreach (var kvp in config.OtrosValores)
            {
                var cell = worksheet.Cell($"{kvp.Value}{row}");
                rowDto.OtrosValores[kvp.Key] = cell.GetString();
            }

            result.Add(rowDto);
            row++;
        }

        return Task.FromResult(result);
    }

    public static Task<ParsedRowLogisticDto> LeerFilasLoigisticDesdeExcelAsync(byte[] fileBytes, ExcelMappingLogisticsConfig config)
    {

        var rowDto = new ParsedRowLogisticDto();

        using var stream = new MemoryStream(fileBytes);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(config.SheetName);
        if (worksheet == null)
            throw new InvalidOperationException($"No se encontró la hoja '{config.SheetName}' en el archivo Excel.");
        
        var contrato = worksheet.Cell(config.Contrato).GetString();

        foreach (var prop in config.Demanda.Fijos.GetType().GetProperties())
        {
            var cellRef = prop.GetValue(config.Demanda.Fijos)?.ToString(); // Ej: "B16"
            if (!string.IsNullOrEmpty(cellRef))
            {
                var cell = worksheet.Cell(cellRef);
                rowDto.Demanda.Fijos[prop.Name] = cell.GetString();
            }
        }

        foreach (var kvp in config.Demanda.ParamentrosCalidad)
        {
            var cell = worksheet.Cell(kvp.Value);
            rowDto.Demanda.ParametrosCalidad[kvp.Key] = cell.GetString();
        }

        var ofertaDict = new Dictionary<string, ParsedOfertaDto>();

        int startRow = config.StartRowOferta;
        int currentRow = startRow;

        while (!worksheet.Row(currentRow).IsEmpty())
        {
            var fijos = new Dictionary<string, string>();
            foreach (var prop in typeof(OfertaColumnasFijasConfig).GetProperties())
            {
                var colStr = (string)prop.GetValue(config.Oferta.Fijos);
                fijos[prop.Name] = worksheet.Cell($"{colStr}{currentRow}").GetString();
            }

            var parametrosCalidad = new Dictionary<string, string>();
            foreach (var kvp in config.Oferta.ParametrosCalidad)
            {
                var colStr = kvp.Value;
                parametrosCalidad[kvp.Key] = worksheet.Cell($"{colStr}{currentRow}").GetString();
            }

            var otrosParamentros = new Dictionary<string, string>();
            foreach (var kvp in config.Oferta.OtrosParamentros)
            {
                var colStr = kvp.Value;
                otrosParamentros[kvp.Key] = worksheet.Cell($"{colStr}{currentRow}").GetString();
            }

            var loteKey = fijos.ContainsKey("Lote") ? fijos["Lote"] : $"Row{currentRow}";

            if (!string.IsNullOrWhiteSpace(loteKey))
            {
                ofertaDict[loteKey] = new ParsedOfertaDto
                {
                    Fijos = fijos,
                    ParametrosCalidad = parametrosCalidad,
                    OtrosParametros = otrosParamentros
                };
            }
            currentRow++;
        }

        var pesoContendoresValue = GetValuePesoContenedores(currentRow, config,worksheet,fileBytes ); 
        
        var finalObject = new ParsedRowLogisticDto
        {
            Demanda = rowDto.Demanda,
            Oferta = ofertaDict,
            Contrato = contrato,
            PesoContenedores = pesoContendoresValue
            
        };

        return Task.FromResult(finalObject);
    }


    public static string GetValuePesoContenedores( int currentRow,  ExcelMappingLogisticsConfig config, IXLWorksheet worksheet,  byte[] fileBytes)
    {
        
        var rowPresoContendores = currentRow + 1;
        
        var cellRefv1 = $"{config.ColumCantidadSacos}{rowPresoContendores}";
        var cellv21 = worksheet.Cell(cellRefv1);
        
        string pesoContenedores;
        if (!string.IsNullOrEmpty(cellv21.FormulaA1))
        {
            using var npoiStream = new MemoryStream(fileBytes);
            IWorkbook wbNPOI = new XSSFWorkbook(npoiStream);
            var sheetNPOI = wbNPOI.GetSheet(config.SheetName);
            var evaluator = wbNPOI.GetCreationHelper().CreateFormulaEvaluator();

            int filaIndex = cellv21.Address.RowNumber - 1;
            int colIndex = cellv21.Address.ColumnNumber - 1;

            var rowNPOI = sheetNPOI.GetRow(filaIndex);
            var cellNPOI = rowNPOI.GetCell(colIndex);

            var evalResult = evaluator.Evaluate(cellNPOI);

            pesoContenedores = evalResult.CellType switch
            {
                CellType.Numeric => evalResult.NumberValue.ToString(),
                CellType.String => evalResult.StringValue,
                CellType.Boolean => evalResult.BooleanValue.ToString(),
                _ => string.Empty
            };
        }
        else
        {
            pesoContenedores = cellv21.GetString();
        }
        
        return pesoContenedores;
    }
}