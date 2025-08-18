using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Agregado;

public class GetAllAgregadosWithoutPaginationFunction
{
    private readonly IMediator _mediator;

    public GetAllAgregadosWithoutPaginationFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Agregado.GetAllWithoutPagination)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.AgregadoBase + "/all")] HttpRequestData req)
    {
        try
        {
            var result = await _mediator.Send(new GetAllAgregadosWithoutPaginationQuery());
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<List<AgregadoDTO>>.Success(result, "Todos los agregados obtenidos correctamente"));
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
