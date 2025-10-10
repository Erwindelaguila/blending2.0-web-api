using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Functions.Support.Authorization;

using Function.Blending.Core.Application.AppParam.Queries;
using Function.Blending.Core.Application.AppParam.DTOs;

using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.AppParam;

public class GetAppParamByIdFunction
{
    private readonly IMediator _mediator;

    public GetAppParamByIdFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [RequireScopes("Administrador,Calidad")]
    [Function(FunctionNames.AppParam.GetById)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Get, Route = ApiRoutes.Core.AppParam.GetById + "/{key}")] HttpRequestData req,
        string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<AppParamDTO>.Fail(
                    "Key parameter is required",
                    "El parámetro 'key' es requerido",
                    400
                ));
            }

            var queryCommand = new GetAppParamByKeyQuery(key);
            
            var result = await _mediator.Send(queryCommand);
            
            if (result == null)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<AppParamDTO>.Fail(
                    $"Parámetro de aplicación con clave '{key}' no encontrado",
                    null,
                    404
                ));
            }
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<AppParamDTO>.Success(
                result, 
                "Parámetro de aplicación obtenido correctamente"
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
