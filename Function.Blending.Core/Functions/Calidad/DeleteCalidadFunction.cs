
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Calidad;

public class DeleteCalidadFunction
{
    private readonly IMediator _mediator;

    public DeleteCalidadFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Calidad.Delete)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Delete, Route = ApiRoutes.Core.Production.CalidadBase + "/{id}")] HttpRequestData req,
        string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var calidadId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID inválido",
                    "El ID debe ser un GUID válido",
                    400
                ));
            }

            var command = new DeleteCalidadCommand(calidadId, req);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Calidad no encontrada",
                    null,
                    404
                ));
            }

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<bool>.Success(result, "Calidad eliminada exitosamente"));
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
            AuthorizationService.ClearCurrentRequestHeaders();
        }
    }
}
