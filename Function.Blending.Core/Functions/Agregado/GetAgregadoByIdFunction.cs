using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Web;

namespace Function.Blending.Core.Functions.Agregado;

public class GetAgregadoByIdFunction
{
    private readonly IMediator _mediator;

    public GetAgregadoByIdFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Agregado.GetById)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.AgregadoGetById)] HttpRequestData req)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            var idString = query["id"];
            if (string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out var agregadoId))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "ID de agregado inválido o no proporcionado",
                    null,
                    400
                ));
            }

            var command = new GetAgregadoByIdQuery(agregadoId, req);
            var result = await _mediator.Send(command);

            if (result == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                    "Agregado no encontrado",
                    null,
                    404
                ));
            }

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<AgregadoDTO>.Success(result, "Agregado obtenido correctamente"));
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
