using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Calidad.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Web;

namespace Function.Blending.Core.Functions.Calidad;

public class GetAllCalidadFunction
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetAllCalidadFunction> _logger;

    public GetAllCalidadFunction(IMediator mediator, ILogger<GetAllCalidadFunction> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function(FunctionNames.Calidad.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.CalidadBase)] HttpRequestData req)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            
            // Log para debug - ver qué parámetros llegan
            _logger.LogInformation("GetAllCalidades called with query: {QueryString}", req.Url.Query);
            
            // Obtener parámetros de paginación con valores por defecto
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
                
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10; // Por defecto 10 registros por página

            // Parsear filtros desde query parameters - puede lanzar ArgumentException
            var filters = QueryParameterHelper.ParseCalidadFilters(query);
            
            // Log para debug - ver qué filtros se parsearon
            _logger.LogInformation("Parsed filters - Codigo: {Codigo}, Estado: {Estado}, FechaDesde: {FechaDesde}", 
                filters.Codigo, filters.Estado, filters.FechaDesde);
            
            // Solo enviar filtros si al menos uno está activo
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var result = await _mediator.Send(new GetAllCalidadesQuery(page, size, filtersToApply));
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<PagedResponse<CalidadDTO>>.Success(result, "Calidades obtenidas correctamente"));
        }
        catch (ArgumentException ex)
        {
            // Error de validación (parámetros inválidos) - devolver 400
            _logger.LogWarning("Validation error in GetAllCalidades: {Message}", ex.Message);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                null,
                400
            ));
        }
        catch (Exception ex)
        {
            // Error interno del servidor - devolver 500
            _logger.LogError(ex, "Unexpected error in GetAllCalidades");
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