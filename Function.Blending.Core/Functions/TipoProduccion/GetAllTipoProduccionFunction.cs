using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.TipoProduccion;

public class GetAllTipoProduccionFunction
{
    private readonly IMediator _mediator;

    public GetAllTipoProduccionFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.TipoProduccion.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.TipoProduccionBase)] HttpRequestData req)
    {
        try
        {
            var result = await _mediator.Send(new GetAllTipoProduccionQuery());
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<List<TipoProduccionDTO>>.Success(result, "Tipos de producción obtenidos correctamente"));
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
