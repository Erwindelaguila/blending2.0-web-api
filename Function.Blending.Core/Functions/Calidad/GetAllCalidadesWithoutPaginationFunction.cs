using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Calidad.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Calidad;

public class GetAllCalidadesWithoutPaginationFunction
{
    private readonly IMediator _mediator;

    public GetAllCalidadesWithoutPaginationFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Calidad.GetAllWithoutPagination)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.CalidadBase + "/all")] HttpRequestData req)
    {
        try
        {
            var result = await _mediator.Send(new GetAllCalidadesWithoutPaginationQuery());
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<List<CalidadDTO>>.Success(result, "Todas las calidades obtenidas correctamente"));
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
