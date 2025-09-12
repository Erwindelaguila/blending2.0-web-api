using System.Web;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.AppParam.Queries;
using Function.Blending.Core.Application.AppParam.DTOs;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.AppParam;

public class GetAllAppParamsFunction
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationHeaderExtractor _headerExtractor; 

    public GetAllAppParamsFunction(IMediator mediator, IAuthorizationHeaderExtractor headerExtractor)
    {
        _mediator = mediator;
        _headerExtractor = headerExtractor;
    }

    [Function(FunctionNames.AppParam.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.AppParam.Base)] HttpRequestData req)
    {
        try
        {
         
            var jwtToken = _headerExtractor.ExtractJwtToken(req);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                AuthorizationService.SetCurrentJwtToken(jwtToken);
                AuthorizationService.SetCurrentRequestData(req);
            }

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
        finally
        {
   
            AuthorizationService.ClearCurrentContext();
        }
    }
}
