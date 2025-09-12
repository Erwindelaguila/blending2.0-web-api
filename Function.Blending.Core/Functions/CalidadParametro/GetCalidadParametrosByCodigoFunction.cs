using Function.Blending.Core.Application.CalidadParametro.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Function.Blending.Core.Functions.CalidadParametro;

public class GetCalidadParametrosByCodigoFunction
{
    private readonly ILogger<GetCalidadParametrosByCodigoFunction> _logger;
    private readonly IMediator _mediator;
    private readonly IAuthorizationHeaderExtractor _headerExtractor; // ✅ NUEVO: Inyección para JWT

    public GetCalidadParametrosByCodigoFunction(
        ILogger<GetCalidadParametrosByCodigoFunction> logger,
        IMediator mediator,
        IAuthorizationHeaderExtractor headerExtractor) // ✅ NUEVO: Inyección
    {
        _logger = logger;
        _mediator = mediator;
        _headerExtractor = headerExtractor; // ✅ NUEVO: Asignación
    }

    [Function(FunctionNames.CalidadParametro.GetByCodigo)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Configuraciones.CalidadParametro)] HttpRequestData req)
    {
        // Obtener el parámetro codigoCalidad del query string
        var queryParams = req.Url.Query.TrimStart('?').Split('&')
            .Where(x => !string.IsNullOrEmpty(x))
            .ToDictionary(x => x.Split('=')[0], x => Uri.UnescapeDataString(x.Split('=')[1]));

        try
        {
            // ✅ NUEVO: Establecer contexto JWT al inicio de la función
            var jwtToken = _headerExtractor.ExtractJwtToken(req);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                AuthorizationService.SetCurrentJwtToken(jwtToken);
                AuthorizationService.SetCurrentRequestData(req);
            }

            if (!queryParams.TryGetValue("codigoCalidad", out var codigoCalidad) || string.IsNullOrWhiteSpace(codigoCalidad))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                    BaseResponse<object>.Fail(
                        "Código de calidad es requerido", 
                        "Debe proporcionar el parámetro 'codigoCalidad' en el query string",
                        400));
            }

            _logger.LogInformation("GetCalidadParametrosByCodigoFunction procesando para código: {CodigoCalidad}", codigoCalidad);

            var query = new GetCalidadParametrosByCodigoQuery(codigoCalidad);
            var result = await _mediator.Send(query);

            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<object>.Success(result, $"Parámetros de calidad '{codigoCalidad}' obtenidos exitosamente"));
        }
        catch (Exception ex)
        {
            var codigoParaLog = queryParams.TryGetValue("codigoCalidad", out var codigo) ? codigo : "N/A";
            _logger.LogError(ex, "Error al obtener parámetros de calidad por código: {CodigoCalidad}", codigoParaLog);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<object>.Fail(
                    "Error interno del servidor", 
                    "Ocurrió un error al procesar la solicitud",
                    500));
        }
        finally
        {
            // ✅ NUEVO: Limpiar contexto de autenticación
            AuthorizationService.ClearCurrentContext();
        }
    }
}
