using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Planta.Commands;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Web;

namespace Function.Blending.Core.Functions.Planta;

public class DeletePlantaFunction
{
    private readonly IMediator _mediator;

    public DeletePlantaFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Planta.Delete)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Delete, Route = ApiRoutes.Core.Planta.Base)] HttpRequestData req)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            var idString = query["id"];
            
            if (string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out var plantaId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID de planta inválido o no proporcionado",
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

            var result = await _mediator.Send(new DeletePlantaCommand(plantaId, eliminadoPorId));
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, result);
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
