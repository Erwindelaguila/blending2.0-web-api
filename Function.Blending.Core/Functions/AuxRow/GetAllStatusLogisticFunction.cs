using Function.Blending.Core.Application.AuxRow.DTOs;
using Function.Blending.Core.Application.AuxRow.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Functions.Support.Authorization;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.AuxRow;

public class GetAllStatusLogisticFunction
{
    private readonly IMediator _mediator;

    public GetAllStatusLogisticFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [RequireScopes("Administrador,Calidad,Logistica")]
    [Function(nameof(GetAllStatusLogisticFunction))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Get, Route = ApiRoutes.Core.AuxRow.StatusLogistic)]
        HttpRequestData req)
    {
        try
        {
            var queryRequest = new GetAllStatusLogisticQuery();

            var result = await _mediator.Send(queryRequest);
            return await HttpResponseHelper.WriteBaseResponseAsync(req,
                BaseResponse<List<StatusRowDTO>>.Success(result ?? new List<StatusRowDTO>(),
                    "Estados de calidad obtenidos correctamente"));
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