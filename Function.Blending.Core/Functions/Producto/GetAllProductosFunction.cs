using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Producto.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Functions.Support.Authorization;


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
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [RequireScopes("Administrador")]
    [Function(FunctionNames.Producto.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Get, Route = ApiRoutes.Core.Production.ProductoBase)] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("GetAllProductosFunction procesando...");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

            // Verificar si es solicitud de activos (para combos)
            if (query["activo"] == "true")
            {
                _logger.LogInformation("Returning active productos for combo");
                var activosResult = await _mediator.Send(new GetAllProductosQuery(1, 1000, null));
                var activosFiltered = activosResult.Items.Where(p => p.Activo).ToList();
                return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                    BaseResponse<object>.Success(activosFiltered, "Productos activos obtenidos correctamente"));
            }

            // Obtener parámetros de paginación con valores por defecto
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
                
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10;

            var filters = QueryParameterHelper.ParseProductoFilters(query);
            
            // Solo aplicar filtros si hay filtros activos
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var getAllQuery = new GetAllProductosQuery(page, size, filtersToApply);

            var result = await _mediator.Send(getAllQuery);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<object>.Success(result, "Productos obtenidos correctamente"));
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
