using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Producto.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Producto.DTOs;
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
            _logger.LogInformation("GetAllProductosFunction procesando... Query: {Query}", req.Url.Query);

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

            // Endpoint para combos (activos solamente)
            if (query["activo"] == "true")
            {
                _logger.LogInformation("Returning active productos for combo");
                // Reutilizamos el handler sin paginación para minimizar duplicación.
                var activosFilters = new ProductoFilterDTO { Estado = "1" }; // Estado 1 => activos
                var activosResult = await _mediator.Send(new GetAllProductoWithoutPaginationQuery(activosFilters));
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(activosResult, "Productos activos obtenidos correctamente"));
            }

            // Parámetros de paginación (defaults)
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10;

            var filters = QueryParameterHelper.ParseProductoFilters(query);
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var getAllQuery = new GetAllProductoWithPaginationQuery(page, size, filtersToApply);
            var result = await _mediator.Send(getAllQuery);

            return await HttpResponseHelper.WriteBaseResponseAsync(req,
                BaseResponse<PagedResponse<ProductoDTO>>.Success(result, "Productos obtenidos correctamente"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error in GetAllProductosFunction: {Message}", ex.Message);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                null,
                400
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetAllProductosFunction: {Message}", ex.Message);
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
