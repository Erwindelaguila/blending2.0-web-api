using System.Text.Json.Nodes;
using ClosedXML.Excel;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Infrastructure.Config.Output;

namespace Function.Blending.Upload.Services.Write.Quality;

public class ExcelWriteQualityService
{
    public static void Execute(
        IXLWorksheet hoja,
        List<Dictionary<string, object>> data,
        Dictionary<string, ExcelConfig> parametros,
        int startRow,
        bool usarItemKeyEnColumnaA = false 
    )
    {
        foreach (var param in parametros)
        {
            var key = param.Key;
            var config = param.Value;

            // Header
            hoja.Cell($"{config.Column}{startRow}").Value = config.Header;
            var row = startRow + 1;

            foreach (var entry in data)
            {
                if (!entry.ContainsKey(key)) continue;

                var value = entry[key];
                var expectedType = (config.DataType ?? "").ToLower();

                try
                {
                    switch (expectedType)
                    {
                        case "string":
                            hoja.Cell($"{config.Column}{row}").Value = value?.ToString() ?? "";
                            break;

                        case "number":
                            if (value is JsonValue jvNumber &&
                                jvNumber.TryGetValue<double>(out var dbl))
                            {
                                hoja.Cell($"{config.Column}{row}").Value = dbl;
                            }
                            else if (double.TryParse(value?.ToString(), out var number))
                            {
                                hoja.Cell($"{config.Column}{row}").Value = number;
                            }
                            else
                            {
                                hoja.Cell($"{config.Column}{row}").Value = value?.ToString() ?? "";
                            }
                            break;

                        default:
                            hoja.Cell($"{config.Column}{row}").Value = value?.ToString() ?? "";
                            break;
                    }
                }
                catch
                {
                    hoja.Cell($"{config.Column}{row}").Value = value?.ToString() ?? "";
                }

                row++;
            }
        }
    }
}
