using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Multipart;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Response;
using Function.Blending.Upload.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions;

public class UploadExcelLogisticsFuncion
{
    private readonly ILogger _logger;
    private readonly XlsmProcessingService<ExcelMappingLogisticsConfig> _xlsmProcessingService;

    public UploadExcelLogisticsFuncion(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<UploadExcelLogisticsFuncion>();
        _xlsmProcessingService = new XlsmProcessingService<ExcelMappingLogisticsConfig>("Templates" ,"ExcelMappingInputLogistic.yaml" );
    }

    [Function("UploadExcelLogisticsFuncion")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload/upload-excel-logistics")] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("Solicitud de carga de archivo recibida.");
        
            if (!MultipartRequestValidator.IsMultipartFormData(req))
            {
                throw new InvalidOperationException($"El tipo de contenido debe ser multipart/form-data.");
            }
            var fileBytes = await MultipartFormDataHelper.ExtractFileAsync(req);
        
            var dataLogistic = await _xlsmProcessingService.ProcesarArchivoLogistcAsync(fileBytes);

            var config = _xlsmProcessingService.ConfiguracionActual;

            var dataFinalParced = ParsedRowMapperHelper.MapearReporte(dataLogistic, config);
        
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<ReporteLogisticDto>.Success(dataFinalParced, "Archivo procesado correctamente."));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando archivo excel de calidad");
            var errorMessage = new
            {
                Message = "Ocurrió un error inesperado.",
                Exception = ex.Message,
                InnerException = ex.InnerException?.Message,
                TraceContext = ex.StackTrace  
            };
            //El dev errorMessage muestra todo los errores asociados
            //En produccion errorMessage puede ser null
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                errorMessage,
                ex.Message,
                500
            ));
        }

    }
    
}