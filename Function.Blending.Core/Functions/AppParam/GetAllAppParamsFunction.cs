using System.Web;
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

public class GetAllAppParamsFunction
{
    private readonly IMediator _mediator;

    public GetAllAppParamsFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [RequireScopes("Administrador,Calidad")]
    [Function(FunctionNames.AppParam.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Get, Route = ApiRoutes.Core.AppParam.Base)] HttpRequestData req)
    {
        try
        {
            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            
            
            var isGlobal = queryParams.Get("isGlobal") ?? "0";

            if (isGlobal != "0" && isGlobal != "1")
            {
                throw new ArgumentException("El parámetro 'isHarina' debe ser '0' o '1'.");
            }

            // Aquí puedes convertirlo a bool si quieres
            bool isGlobalConfig = isGlobal == "1";
            
            var page = int.TryParse(queryParams["page"], out var p) ? p : 1;
            var size = int.TryParse(queryParams["size"], out var s) ? s : 10;
            var filters = QueryParameterHelper.ParseAppParamFilters(queryParams);
            
            var query = new GetAllAppParamsQuery(page, size, filters, isGlobalConfig);
            
            var result = await _mediator.Send(query);


            if (isGlobalConfig)
            {
                // Validar que AppParamShortList no sea null
                if (result.AppParamShortList == null)
                {
                    throw new InvalidOperationException("AppParamShortList es null cuando debería tener datos");
                }

                var responseShortList = BaseResponse<List<AppParamSortDTO>>.Success(
                    result.AppParamShortList, 
                    "AppParams obtenidos exitosamente"
                );

                return await HttpResponseHelper.WriteBaseResponseAsync(req, responseShortList);
            }
            
            // Validar que AppParamPaginate no sea null
            if (result.AppParamPaginate == null)
            {
                throw new InvalidOperationException("AppParamPaginate es null cuando debería tener datos");
            }

            var responsePaginate = BaseResponse<PagedResponse<AppParamDTO>>.Success(
                result.AppParamPaginate, 
                "AppParams obtenidos exitosamente"
            );

            return await HttpResponseHelper.WriteBaseResponseAsync(req, responsePaginate);
            
            

            
        }
        catch (Exception ex)
        {
            var errorMessage = new
            {
                Message = "Ocurrió un error inesperado.",
                Exception = ex.Message,
                InnerException = ex.InnerException?.Message,
            };
            
            var errorResponse = BaseResponse<PagedResponse<AppParamDTO>>.Fail(
                errorMessage,
                null,
                500
            );
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, errorResponse);
        }
   
    }
}
