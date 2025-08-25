using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.AppParam.Queries;
using Function.Blending.Core.Application.AppParam.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.AppParam;

public class GetAppParamsByGroupFunction
{
    private readonly IMediator _mediator;

    public GetAppParamsByGroupFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.AppParam.GetByGroup)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.AppParam.GetByGroup + "/{group}")] HttpRequestData req,
        string group)
    {
        try
        {
            var query = new GetAppParamsByGroupQuery(group);
            
            var result = await _mediator.Send(query);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<IEnumerable<AppParamDTO>>.Success(
                result, 
                "AppParams por grupo obtenidos exitosamente"
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
