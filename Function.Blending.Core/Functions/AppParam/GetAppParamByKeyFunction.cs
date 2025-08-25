using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.AppParam.Queries;
using Function.Blending.Core.Application.AppParam.DTOs;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.AppParam;

public class GetAppParamByKeyFunction
{
    private readonly IMediator _mediator;

    public GetAppParamByKeyFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.AppParam.GetByKey)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.AppParam.GetByKey + "/{key}")] HttpRequestData req,
        string key)
    {
        try
        {
            var query = new GetAppParamByKeyQuery(key);
            
            var result = await _mediator.Send(query);
            
            if (result == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<AppParamDTO>.Fail(
                    $"AppParam with key '{key}' not found",
                    "AppParam no encontrado",
                    404
                ));
            }
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<AppParamDTO>.Success(
                result, 
                "AppParam obtenido exitosamente"
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
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<AppParamDTO>.Fail(
                errorMessage,
                null,
                500
            ));
        }
    }
}
