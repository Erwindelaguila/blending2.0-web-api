using System.Net;
using Function.Blending.Upload.Functions.Process;
using Function.Blending.Upload.Functions.Support.Routing;
using Function.Blending.Upload.Helpers.Http;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Response;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;


public class GetSapStockFunction
{
    private readonly ILogger _logger;
    private readonly SapStockProcess _sapStockProcess;

    public GetSapStockFunction(
        ILogger<GetSapStockFunction> logger,
        SapStockProcess sapStockProcess
    )
    {
        _sapStockProcess = sapStockProcess;
        _logger = logger;
    }

    [Function(nameof(GetSapStockFunction))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = FunctionRoutes.SapStock.get)]
        HttpRequestData req)
    {
        try
        {
            var blobResult = await _sapStockProcess.ExecuteAsync(req);
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