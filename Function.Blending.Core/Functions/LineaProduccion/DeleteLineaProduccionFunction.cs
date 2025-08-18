using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Web;

namespace Function.Blending.Core.Functions.LineaProduccion;

public class DeleteLineaProduccionFunction
{
    private readonly IMediator _mediator;

    public DeleteLineaProduccionFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.LineaProduccion.Delete)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Delete, Route = ApiRoutes.Core.Production.LineaProduccionBase)] HttpRequestData req)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            var idString = query["id"];

            if (string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out var lineaProduccionId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID de línea de producción inválido o no proporcionado",
                    null,
                    400
                ));
            }

            var eliminadoPorIdString = query["eliminadoPorId"];
            if (string.IsNullOrEmpty(eliminadoPorIdString) || !Guid.TryParse(eliminadoPorIdString, out var eliminadoPorId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID de usuario eliminador inválido o no proporcionado",
                    null,
                    400
                ));
            }

            var result = await _mediator.Send(new DeleteLineaProduccionCommand(lineaProduccionId, eliminadoPorId));

            if (!result)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Línea de producción no encontrada",
                    null,
                    404
                ));
            }

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<bool>.Success(result, "Línea de producción eliminada exitosamente"));
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
