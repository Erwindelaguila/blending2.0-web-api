using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Agregado.Commands;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Agregado;

public class DeleteAgregadoFunction
{
    private readonly IMediator _mediator;

    public DeleteAgregadoFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Agregado.Delete)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Delete, Route = ApiRoutes.Core.Production.AgregadoBase + "/{id}")] HttpRequestData req,
        string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var agregadoId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID inválido",
                    "El ID debe ser un GUID válido",
                    400
                ));
            }

            var command = new DeleteAgregadoCommand(agregadoId, req);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Agregado no encontrado",
                    null,
                    404
                ));
            }

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<bool>.Success(result, "Agregado eliminado exitosamente"));
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
    }
}
