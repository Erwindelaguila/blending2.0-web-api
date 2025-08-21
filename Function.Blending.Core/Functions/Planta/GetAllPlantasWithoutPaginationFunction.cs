using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Planta.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Planta;

public class GetAllPlantasWithoutPaginationFunction
{
    private readonly IMediator _mediator;

    public GetAllPlantasWithoutPaginationFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Planta.GetAllWithoutPagination)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Planta.Base + "/all")] HttpRequestData req)
    {
        try
        {
            var result = await _mediator.Send(new GetAllPlantasWithoutPaginationQuery());
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<List<PlantaDTO>>.Success(result, "Todas las plantas obtenidas correctamente"));
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
