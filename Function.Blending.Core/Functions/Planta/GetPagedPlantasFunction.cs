using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Planta.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Planta;

public class GetPagedPlantasFunction
{
    private readonly IMediator _mediator;

    public GetPagedPlantasFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.Planta.GetPaged)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Planta.Paged)] HttpRequestData req)
    {
        try
        {
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            
            if (!int.TryParse(query["page"], out var page) || page <= 0)
                page = 1;
            
            if (!int.TryParse(query["size"], out var size) || size <= 0 || size > 100)
                size = 10;

            var result = await _mediator.Send(new GetPagedPlantasQuery(page, size));
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(result, "Plantas obtenidas exitosamente"));
        }
        catch (Exception ex)
        {
            var errorMessage = new
            {
                Message = "Ocurrió un error inesperado.",
                Exception = ex.Message,
                InnerException = ex.InnerException?.Message
            };
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                errorMessage,
                null,
                500
            ));
        }
    }
}
