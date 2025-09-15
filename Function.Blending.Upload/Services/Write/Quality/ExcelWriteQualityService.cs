using System.Text.Json.Nodes;
using ClosedXML.Excel;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Infrastructure.Config.Output;

namespace Function.Blending.Upload.Services.Write.Quality;

public class ExcelWriteQualityService
{
    public static void Execute<T>(
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
}