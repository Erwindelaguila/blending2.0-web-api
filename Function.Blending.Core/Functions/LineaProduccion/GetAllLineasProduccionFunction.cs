using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.LineaProduccion.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Web;

namespace Function.Blending.Core.Functions.LineaProduccion;

public class GetAllLineasProduccionFunction
{
    private readonly IMediator _mediator;

    public GetAllLineasProduccionFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(FunctionNames.LineaProduccion.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.LineaProduccionBase)] HttpRequestData req)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            
            var pageString = query["page"];
            var sizeString = query["size"];
            
            var page = int.TryParse(pageString, out var parsedPage) ? parsedPage : 1;
            var size = int.TryParse(sizeString, out var parsedSize) ? parsedSize : 10;
            
            var getAllQuery = new GetAllLineasProduccionQuery
            {
                Page = page,
                Size = size
            };
            
            var result = await _mediator.Send(getAllQuery);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(result, "Líneas de producción obtenidas correctamente"));
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
