using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Web;

namespace Function.Blending.Core.Functions.TipoProduccion;

/// <summary>
/// Función para obtener tipo de producción por ID
/// Implementa patrón clean code con manejo de errores estandarizado
/// </summary>
public class GetTipoProduccionByIdFunction
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationHeaderExtractor _headerExtractor; // ✅ NUEVO: Inyección para JWT

    public GetTipoProduccionByIdFunction(
        IMediator mediator,
        IAuthorizationHeaderExtractor headerExtractor) // ✅ NUEVO: Inyección
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _headerExtractor = headerExtractor ?? throw new ArgumentNullException(nameof(headerExtractor)); // ✅ NUEVO: Asignación
    }

    [Function(FunctionNames.TipoProduccion.GetById)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Get, Route = ApiRoutes.Core.Production.TipoProduccionGetById)] HttpRequestData req)
    {
        try
        {
            // ✅ NUEVO: Establecer contexto JWT al inicio de la función
            var jwtToken = _headerExtractor.ExtractJwtToken(req);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                AuthorizationService.SetCurrentJwtToken(jwtToken);
                AuthorizationService.SetCurrentRequestData(req);
            }

            var query = HttpUtility.ParseQueryString(req.Url.Query);
            var idString = query["id"];
            
            if (string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out var tipoProduccionId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID de tipo de producción inválido o no proporcionado",
                    null,
                    400
                ));
            }

            var result = await _mediator.Send(new GetTipoProduccionByIdWithRelationsQuery(tipoProduccionId));

            if (result == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Tipo de producción no encontrado",
                    null,
                    404
                ));
            }

            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<TipoProduccionDTO>.Success(result, "Tipo de producción obtenido correctamente"));
        }
        catch (Exception ex)
        {
            var errorMessage = new
            {
                Message = "Ocurrió un error inesperado.",
                Exception = ex.Message,
                InnerException = ex.InnerException?.Message,
            };
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                errorMessage,
                null,
                500
            ));
        }
        finally
        {
            // ✅ NUEVO: Limpiar contexto de autenticación
            AuthorizationService.ClearCurrentContext();
        }
    }
}
