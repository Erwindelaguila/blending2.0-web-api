using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.TipoProduccion;

/// <summary>
/// Función para eliminar tipos de producción
/// Implementa auditoría automática y validaciones de negocio
/// </summary>
public class DeleteTipoProduccionFunction
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationHeaderExtractor _headerExtractor; // ✅ NUEVO: Inyección para JWT

    public DeleteTipoProduccionFunction(
        IMediator mediator,
        IAuthorizationHeaderExtractor headerExtractor) // ✅ NUEVO: Inyección
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _headerExtractor = headerExtractor ?? throw new ArgumentNullException(nameof(headerExtractor)); // ✅ NUEVO: Asignación
    }

    [Function(FunctionNames.TipoProduccion.Delete)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Delete, Route = ApiRoutes.Core.Production.TipoProduccionBase + "/{id}")] HttpRequestData req,
        string id)
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

            if (!Guid.TryParse(id, out var tipoProduccionId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID inválido",
                    "El ID debe ser un GUID válido",
                    400
                ));
            }

            var command = new DeleteTipoProduccionCommand(tipoProduccionId, req);

            var result = await _mediator.Send(command);

            if (!result)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Tipo de producción no encontrado",
                    "Recurso no encontrado",
                    404
                ));
            }

            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<bool>.Success(result, "Tipo de producción eliminado exitosamente"));
        }
        catch (EntityInUseException ex)
        {
            var error = new { Message = ex.Message };
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                error,
                "No se puede eliminar",
                400
            ));
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
