using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Parametro.Queries;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Parametro;

public class GetAllParametrosWithoutPaginationFunction
{
    private readonly IMediator _mediator;

    public GetAllParametrosWithoutPaginationFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Parametro.GetAllWithoutPagination)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Parametro.Base + "/all")] HttpRequestData req)
    {
        try
        {
            var queryRequest = new GetAllParametrosWithoutPaginationQuery(req);
            var result = await _mediator.Send(queryRequest);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<List<ParametroDTO>>.Success(result, "Todos los parámetros obtenidos correctamente"));
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
