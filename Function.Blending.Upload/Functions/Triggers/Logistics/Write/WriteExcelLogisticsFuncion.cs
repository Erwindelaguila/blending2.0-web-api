using System.Net;
using Function.Blending.Upload.Functions.Process;
using Function.Blending.Upload.Functions.Support.Routing;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Http;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Response;
using Function.Blending.Upload.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions;

public class WriteExcelLogisticsFuncion
{
    private readonly ILogger<WriteExcelLogisticsFuncion> _logger;
    private readonly WriteExcelLogisticsProcess _writeExcelLogisticsProcess;

    public WriteExcelLogisticsFuncion(ILogger<WriteExcelLogisticsFuncion> logger, WriteExcelLogisticsProcess writeExcelLogisticsProcess)
    {
        _logger = logger;
        _writeExcelLogisticsProcess = writeExcelLogisticsProcess;
        
    }

    [Function(nameof(WriteExcelLogisticsFuncion))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FunctionRoutes.Logistic.write)]
        HttpRequestData req)
    {
        try
        {
            var blobResult = await _writeExcelLogisticsProcess.ExecuteAsync(req);
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