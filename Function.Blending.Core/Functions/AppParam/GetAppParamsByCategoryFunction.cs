using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.AppParam.Queries;
using Function.Blending.Core.Application.AppParam.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.AppParam;

public class GetAppParamsByCategoryFunction
{
    private readonly IMediator _mediator;

    public GetAppParamsByCategoryFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.AppParam.GetByCategory)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.AppParam.GetByCategory + "/{category}")] HttpRequestData req,
        string category)
    {
        try
        {
            var query = new GetAppParamsByCategoryQuery(category);
            
            var result = await _mediator.Send(query);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<IEnumerable<AppParamDTO>>.Success(
                result, 
                "AppParams por categoría obtenidos exitosamente"
            ));
        }
        catch (Exception ex)
        {
            var errorMessage = new
            {
                Message = "Ocurrió un error inesperado.",
                Exception = ex.Message,
                InnerException = ex.InnerException?.Message,
            };
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<IEnumerable<AppParamDTO>>.Fail(
                errorMessage,
                null,
                500
            ));
        }
    }
}
