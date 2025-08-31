using System.Web;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
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

    public GetAllAppParamsFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.AppParam.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.AppParam.Base)] HttpRequestData req)
    {
        try
        {
            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            
            var page = int.TryParse(queryParams["page"], out var p) ? p : 1;
            var size = int.TryParse(queryParams["size"], out var s) ? s : 10;
            var filters = QueryParameterHelper.ParseAppParamFilters(queryParams);
            
            var query = new GetAllAppParamsQuery(page, size, filters);
            
            var result = await _mediator.Send(query);
            
            var response = BaseResponse<PagedResponse<AppParamDTO>>.Success(
                result, 
                "AppParams obtenidos exitosamente"
            );
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, response);
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
            AuthorizationService.ClearCurrentRequestHeaders();
        }
    }
}
