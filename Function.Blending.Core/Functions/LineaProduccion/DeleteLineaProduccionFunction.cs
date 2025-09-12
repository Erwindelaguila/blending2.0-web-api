using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.LineaProduccion;

public class DeleteLineaProduccionFunction
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationHeaderExtractor _headerExtractor;

    public DeleteLineaProduccionFunction(IMediator mediator, IAuthorizationHeaderExtractor headerExtractor)
    {
        _mediator = mediator;
        _headerExtractor = headerExtractor;
    }

    [Function(FunctionNames.LineaProduccion.Delete)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Delete, Route = ApiRoutes.Core.Production.LineaProduccionBase + "/{id}")] HttpRequestData req,
        string id)
    {
        try
        {
        
            var jwtToken = _headerExtractor.ExtractJwtToken(req);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                AuthorizationService.SetCurrentJwtToken(jwtToken);
                AuthorizationService.SetCurrentRequestData(req);
            }

            if (!Guid.TryParse(id, out var lineaProduccionId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID inválido",
                    "El ID debe ser un GUID válido",
                    400
                ));
            }

            var command = new DeleteLineaProduccionCommand(lineaProduccionId, req);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Línea de Producción no encontrada",
                    null,
                    404
                ));
            }

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<bool>.Success(result, "Línea de Producción eliminada exitosamente"));
        }
        catch (EntityInUseException ex)
        {
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                new { Error = ex.Message, Code = ex.ErrorCode },
                "Conflicto de regla de negocio",
                409
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
           
            AuthorizationService.ClearCurrentContext();
        }
    }
}
