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

public class UploadExcelQualityFunction
{
  private readonly ILogger _logger;
  private readonly XlsmProcessingService<ExcelMappingConfig> _xlsmProcessingService;


  public UploadExcelQualityFunction(ILoggerFactory loggerFactory)
  {
    _logger = loggerFactory.CreateLogger<UploadExcelQualityFunction>();
    _xlsmProcessingService = new XlsmProcessingService<ExcelMappingConfig>("Templates" ,"ExcelMappingInput.yaml" );
  }

  [Function("UploadExcelQualityFunction")]
  public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload/upload-excel-quality")] HttpRequestData req)
  {
    try
    {
      _logger.LogInformation("📥 Solicitud de carga de archivo recibida.");

      if (!MultipartRequestValidator.IsMultipartFormData(req))
      {
        throw new InvalidOperationException($"El tipo de contenido debe ser multipart/form-data.");
      }
      //Lee el archivo y devuleve bytes del archivo
      
      var fileBytes = await MultipartFormDataHelper.ExtractFileAsync(req);
      
      var filas = await _xlsmProcessingService.ProcesarArchivoAsync(fileBytes);

      // Mapear filas válidas a DTOs
      var config = _xlsmProcessingService.ConfiguracionActual;
      
      var listaFinal = filas
        .Where(ParsedRowValidator.EsValido)
        .Select(fila => ParsedRowMapperHelper.Mapear(fila, config))
        .ToList();
      
      return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse< List<RumaStockDisponibleDto>>.Success(listaFinal, "Archivo procesado correctamente."));
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
