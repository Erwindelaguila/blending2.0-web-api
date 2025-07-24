using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Agregado;

public class GetAllAgregadosFunction
{
    private readonly IMediator _mediator;

    public GetAllAgregadosFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Agregado.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.AgregadoBase)] HttpRequestData req)
    {
        try
        {
            var result = await _mediator.Send(new GetAllAgregadosQuery());
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<List<AgregadoDTO>>.Success(result, "Agregados obtenidos correctamente"));
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
