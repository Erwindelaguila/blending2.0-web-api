using FunctionBlending.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Response;
using Microsoft.Extensions.Logging;

namespace FunctionBlending.Core.Functions;

public class ObtenerCadmioFunction
{
    private readonly ILogger _logger;
    private readonly CadmioService _cadmioService;


    public ObtenerCadmioFunction(CadmioService cadmioService, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ObtenerCadmioFunction>();
        _cadmioService = cadmioService;
    }

    [Function("ObtenerCadmioFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload/get-cadmio-rumas")]
        HttpRequestData req)
    {
        try
        {
            var requestDto = await req.ReadFromJsonAsync<ObtenerCadmioRequestDto>();

            if (requestDto == null || requestDto.Rumas.Length == 0)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Debe enviar al menos una ruma.");
                return badRequest;
            }

            var result = await _cadmioService.ObtenerCadmioAsync(requestDto);
            return await HttpResponseHelper.WriteBaseResponseAsync(req,
                BaseResponse<List<CadmioResult>>.Success(result, "Archivo procesado correctamente."));
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