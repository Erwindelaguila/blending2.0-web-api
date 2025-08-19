using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.LineaProduccion.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Web;

namespace Function.Blending.Core.Functions.LineaProduccion;

public class GetAllLineasProduccionFunction
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetAllLineasProduccionFunction> _logger;

    public GetAllLineasProduccionFunction(IMediator mediator, ILogger<GetAllLineasProduccionFunction> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function(FunctionNames.LineaProduccion.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.LineaProduccionBase)] HttpRequestData req)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            
            // Log para debug - ver qué parámetros llegan
            _logger.LogInformation("GetAllLineasProduccion called with query: {QueryString}", req.Url.Query);
            
            // Obtener parámetros de paginación con valores por defecto
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
                
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10; // Por defecto 10 registros por página

            // Parsear filtros desde query parameters - puede lanzar ArgumentException
            var filters = QueryParameterHelper.ParseLineaProduccionFilters(query);
            
            // Log para debug - filtros activos
            _logger.LogInformation("Parsed filters - Codigo: {Codigo}, Estado: {Estado}, FechaDesde: {FechaDesde}", 
                filters.Codigo, filters.Estado, filters.FechaDesde);
            
            // Solo enviar filtros si al menos uno está activo
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var result = await _mediator.Send(new GetAllLineasProduccionQuery(page, size, filtersToApply));
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<PagedResponse<LineaProduccionDTO>>.Success(result, "Líneas de producción obtenidas correctamente"));
        }
        catch (ArgumentException ex)
        {
            // Error de validación (parámetros inválidos) - devolver 400
            _logger.LogWarning("Validation error in GetAllLineasProduccion: {Message}", ex.Message);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                null,
                400
            ));
        }
        catch (Exception ex)
        {
            // Error interno del servidor - devolver 500
            _logger.LogError(ex, "Unexpected error in GetAllLineasProduccion");
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
