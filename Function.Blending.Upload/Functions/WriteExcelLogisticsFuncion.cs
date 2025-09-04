using System.Net;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions;

public class WriteExcelLogisticsFuncion
{
    private readonly ILogger _logger;
    private readonly XlsmProcessingService<ExcelMappingOutputLogisticsConfig> _xlsmProcessingService;

    public WriteExcelLogisticsFuncion(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<WriteExcelLogisticsFuncion>();
        _xlsmProcessingService = new XlsmProcessingService<ExcelMappingOutputLogisticsConfig>("Templates", "ExcelMappingOutputLogistic.yml");
    }

    [Function("WriteXlsmLogisticsFuncion")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload/write-excel-logistic")] HttpRequestData req)
    {
        try
        {
            var body = await req.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Request body cannot be null or empty.");

            var json = _xlsmProcessingService.JsonDeserialize(body);
            
            var config = _xlsmProcessingService.ConfiguracionActual;
            var dataContenedoresHomogenizacion = json.Result.Data;
            using var workbook = _xlsmProcessingService.GetXLWorkbookAction("Templates", "template_output_logistica.xlsx");
            
            ExcelWriterService.WriteHomogenizacionContenedores(config, dataContenedoresHomogenizacion, workbook);
            ExcelWriterService.WriteSapLogistic(config, dataContenedoresHomogenizacion, workbook);
            
            
            
            // Guardar el resultado
            var outputPath = Path.Combine(Path.GetTempPath(), "resultadov1.xlsx");
            workbook.SaveAs(outputPath);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Archivo generado: {outputPath}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando archivo .xlsm");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Error: {ex.Message}");
            return errorResponse;
        }
    }
    
    
}
