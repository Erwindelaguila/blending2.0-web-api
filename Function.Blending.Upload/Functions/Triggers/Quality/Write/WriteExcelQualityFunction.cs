using System.Net;
using Function.Blending.Upload.Functions.Process;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Http;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Response;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Triggers.Quality.Write;

public class WriteExcelQualityFunction
{
    private readonly ILogger<WriteExcelQualityFunction> _logger;
    private readonly WriteExcelQualityProcess _writeExcelQualityProcess;
    
    public WriteExcelQualityFunction(ILogger<WriteExcelQualityFunction> logger, WriteExcelQualityProcess writeExcelQualityProcess)
    {
        _logger = logger;
        _writeExcelQualityProcess = writeExcelQualityProcess;
    }

    [Function(nameof(WriteExcelQualityFunction))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "upload/write-excel-quality")] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("WriteExcelQualityFunction function processed a request.");
            var blobResult = await _writeExcelQualityProcess.ExecuteAsync(req);
            return await HttpResponseHelper.WriteBaseResponseAsync(
                req,
                BaseResponse<BlobResultDto>.Success(
                    blobResult,
                    "Datos obtenidos correctamente")
            );
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