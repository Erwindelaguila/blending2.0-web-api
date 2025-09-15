using FunctionBlending.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Function.Blending.Upload.Exceptions;
using Function.Blending.Upload.Functions.Process;
using Function.Blending.Upload.Functions.Support.Routing;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Http;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Response;
using Microsoft.Extensions.Logging;

namespace FunctionBlending.Core.Functions;

public class GetCadmioFunction
{
    private readonly ILogger _logger;
    private readonly GetCadmioProcess _getCadmioProcess;


    public GetCadmioFunction(ILogger<GetCadmioFunction> logger, GetCadmioProcess getCadmioProcess)
    {
        _logger = logger;
        _getCadmioProcess = getCadmioProcess;
    }

    [Function(nameof(GetCadmioFunction))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FunctionRoutes.Cadmio.get)]
        HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("Iniciando la obtencion de cadmio ");
            var resultList = await _getCadmioProcess.ExecuteAsync(req);
            return await HttpResponseHelper.WriteBaseResponseAsync(req,
                BaseResponse<List<CadmioResult>>.Success(resultList, "Archivo procesado correctamente."));
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Error de validación en la request.");
            return await HttpResponseHelper.WriteBaseResponseAsync(
                req,
                BaseResponse<object>.Fail(null, ex.Message, (int)HttpStatusCode.BadRequest)
            );
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado.");
            return await HttpResponseHelper.WriteBaseResponseAsync(
                req,
                BaseResponse<object>.Fail(null, ex.Message, (int)HttpStatusCode.NotFound)
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