using System.Net;
using Function.Blending.Upload.Functions.Process;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Http;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Response;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Triggers.Quality.Upload;

public class UploadExcelQualityFunction
{
  private readonly ILogger<UploadExcelQualityFunction> _logger;
  private readonly UploadExcelQualityProcess _uploadExcelQualityProcess;
  
  public UploadExcelQualityFunction(ILogger<UploadExcelQualityFunction> logger , UploadExcelQualityProcess uploadExcelQualityProcess)
  {
    _logger = logger;
    _uploadExcelQualityProcess = uploadExcelQualityProcess;
  }

  [Function(nameof(UploadExcelQualityFunction))]
  public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload/upload-excel-quality")] HttpRequestData req)
  {
    try
    {
      _logger.LogInformation("📥 Solicitud de carga de archivo calidad recibida.");
      var result = await _uploadExcelQualityProcess.ExecuteAsync(req);
      return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse< List<ExcelExtractQualityDto>>.Success(result, "Archivo procesado correctamente."));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error inesperado en la función GetCadmio");
      return await HttpResponseHelper.WriteBaseResponseAsync(
        req,
        BaseResponse<object>.Fail(null, "Ocurrió un error inesperado.", (int)HttpStatusCode.InternalServerError)
      );
    }
  }
}
