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
            _logger.LogInformation("GetAllProductoWithoutPaginationFunction procesando... Query: {Query}", req.Url.Query);

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var filters = QueryParameterHelper.ParseProductoFilters(query);
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            // Si piden activos (?activo=true) priorizamos ese modo ignorando otros filtros de estado
            if (query["activo"] == "true")
            {
                _logger.LogInformation("Returning active productos (without pagination)");
                filtersToApply = new ProductoFilterDTO { Estado = "1" };
            }

            var getAllQuery = new GetAllProductoWithoutPaginationQuery(filtersToApply);
            var result = await _mediator.Send(getAllQuery);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<List<ProductoDTO>>.Success(result, "Productos obtenidos correctamente"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error in GetAllProductoWithoutPaginationFunction: {Message}", ex.Message);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                null,
                400
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetAllProductoWithoutPaginationFunction: {Message}", ex.Message);
            var errorMessage = new { Message = "Ocurrió un error inesperado.", Exception = ex.Message, InnerException = ex.InnerException?.Message };
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(errorMessage, null, 500));
        }
    }
}
