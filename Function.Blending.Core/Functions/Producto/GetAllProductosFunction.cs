using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Producto.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Net;

namespace Function.Blending.Core.Functions.Producto;

public class GetAllProductosFunction
{
    private readonly ILogger<GetAllProductosFunction> _logger;
    private readonly IMediator _mediator;

    public GetAllProductosFunction(
        ILogger<GetAllProductosFunction> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Function(FunctionNames.Producto.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = ApiRoutes.Core.Production.ProductoBase)] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("GetAllProductosFunction procesando...");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

            // Obtener parámetros de paginación con valores por defecto
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
                
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10;

            var filters = QueryParameterHelper.ParseProductoFilters(query);
            
            // Solo aplicar filtros si hay filtros activos
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var getAllQuery = new GetAllProductoWithPaginationQuery(page, size, filtersToApply);

            var result = await _mediator.Send(getAllQuery);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetAllProductosFunction: {Message}", ex.Message);
            
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "Error interno del servidor" });
            return errorResponse;
        }
    }
}
