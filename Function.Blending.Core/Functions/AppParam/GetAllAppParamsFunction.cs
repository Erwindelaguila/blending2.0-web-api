using System.Web;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
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

    [Function(FunctionNames.AppParam.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.AppParam.Base)] HttpRequestData req)
    {
        try
        {
            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);

            var isGlobalConfig = queryParams.Get("isGlobal") ?? "0";

            if (isGlobalConfig != "0" && isGlobalConfig != "1")
            {
                throw new ArgumentException("El parámetro 'isHarina' debe ser '0' o '1'.");
            }

            // Aquí puedes convertirlo a bool si quieres
            bool globalConfig = isGlobalConfig == "1";


            var query = new GetAllAppParamsQuery
            {
                Page = int.TryParse(queryParams["page"], out var page) ? page : 1,
                Size = int.TryParse(queryParams["size"], out var size) ? size : 10,
                Key = queryParams["key"],
                IsActive = bool.TryParse(queryParams["isActive"], out var isActive) ? isActive : null,
                Fecha = DateTime.TryParse(queryParams["fecha"], out var fecha) ? fecha : null,
                GlobalConfig = globalConfig
            };

            var result = await _mediator.Send(query);

            if (globalConfig)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<List<AppParamSortDTO>>.Success(
                    result.AppParamShortList,
                    "AppParams obtenidos exitosamente"
                ));
            }

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<PagedResponse<AppParamDTO>>.Success(
                result.AppParamPaginate,
                "AppParams obtenidos exitosamente"
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

            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<PagedResponse<AppParamDTO>>.Fail(
                errorMessage,
                null,
                500
            ));
        }
    }
}
