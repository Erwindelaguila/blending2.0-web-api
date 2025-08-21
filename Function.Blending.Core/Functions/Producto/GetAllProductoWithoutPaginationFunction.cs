using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Producto.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Net;

namespace Function.Blending.Core.Functions.Producto;

public class GetAllProductoWithoutPaginationFunction
{
    private readonly ILogger<GetAllProductoWithoutPaginationFunction> _logger;
    private readonly IMediator _mediator;

    public GetAllProductoWithoutPaginationFunction(
        ILogger<GetAllProductoWithoutPaginationFunction> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Function(FunctionNames.Producto.GetAllWithoutPagination)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "productos/without-pagination")] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("GetAllProductoWithoutPaginationFunction procesando...");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var filters = QueryParameterHelper.ParseProductoFilters(query);
            
            // Solo aplicar filtros si hay filtros activos
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var getAllQuery = new GetAllProductoWithoutPaginationQuery(filtersToApply);

            var result = await _mediator.Send(getAllQuery);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetAllProductoWithoutPaginationFunction: {Message}", ex.Message);
            
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "Error interno del servidor" });
            return errorResponse;
        }
    }
}
