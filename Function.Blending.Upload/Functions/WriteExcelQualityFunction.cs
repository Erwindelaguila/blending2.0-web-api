using ClosedXML.Excel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json.Nodes;
using Function.Blending.Upload.Services;


public class WriteExcelQualityFunction
{
    private readonly ILogger _logger;

    public WriteExcelQualityFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<WriteExcelQualityFunction>();
    }

    [Function("ReadXlsmFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "upload/write-excel-quality")] HttpRequestData req)
    {
        try
        {
            var _xlsmReadingService = new XlsmWritingService();
            _logger.LogInformation("Procesando archivo ...");
            var config = _xlsmReadingService.ConfiguracionActual;
            var configJsonData = await _xlsmReadingService.JsonDeserialize();
            using var workbook = _xlsmReadingService.GetXLWorkbook();

            // Hoja de Resumen
            var hojaResumen = workbook.Worksheet(config.Resumen.SheetName);
            var startRowResumen = config.Resumen.StartRow;

            ExcelWriterService.Write<JsonObject>(hojaResumen, configJsonData.resultado.grupos, config.Resumen.Fijos, startRowResumen, true);
            ExcelWriterService.Write<JsonObject>(hojaResumen, configJsonData.resultado.grupos, config.Resumen.ParametrosCalidad, startRowResumen, true);

            // Hoja Detalle
            var hojaDetalle = workbook.Worksheet(config.Detalle.SheetName);
            var startRowDetalle = config.Detalle.StartRow;

            ExcelWriterService.Write<JsonObject>(hojaDetalle, configJsonData.resultado.rumas, config.Detalle.Fijos, startRowDetalle); 
            ExcelWriterService.Write<JsonObject>(hojaDetalle, configJsonData.resultado.rumas, config.Detalle.ParametrosCalidad, startRowDetalle);

            var outputPath = Path.Combine(Path.GetTempPath(), "resultado.xlsx");
            workbook.SaveAs(outputPath);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Archivo generado: {outputPath}");
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error procesando el archivo");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Ocurrió un error procesando el archivo.{e}");
            return errorResponse;
        }
    }
}