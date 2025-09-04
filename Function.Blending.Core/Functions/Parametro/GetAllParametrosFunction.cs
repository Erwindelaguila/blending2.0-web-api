using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Parametro.Queries;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Web;

namespace Function.Blending.Core.Functions.Parametro;

public class GetAllParametrosFunction
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetAllParametrosFunction> _logger;

    public GetAllParametrosFunction(IMediator mediator, ILogger<GetAllParametrosFunction> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function(FunctionNames.Parametro.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Parametro.Base)] HttpRequestData req)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            
            // Log para debug - ver qué parámetros llegan
            _logger.LogInformation("GetAllParametros called with query: {QueryString}", req.Url.Query);
            
            // Obtener parámetros de paginación con valores por defecto
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
                
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10; // Por defecto 10 registros por página

            // Parsear filtros desde query parameters - puede lanzar ArgumentException
            var filters = QueryParameterHelper.ParseParametroFilters(query);
            
            // Log para debug - ver qué filtros se parsearon
            _logger.LogInformation("Parsed filters - Codigo: {Codigo}, Estado: {Estado}, FechaDesde: {FechaDesde}", 
                filters.Codigo, filters.Estado, filters.FechaDesde);
            
            // Solo enviar filtros si al menos uno está activo
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var queryRequest = new GetAllParametrosQuery(page, size, filtersToApply, req);
            var result = await _mediator.Send(queryRequest);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<PagedResponse<ParametroDTO>>.Success(result, "Parámetros obtenidos correctamente"));
        }
        catch (ArgumentException ex)
        {
            // Error de validación (parámetros inválidos) - devolver 400
            _logger.LogWarning("Validation error in GetAllParametros: {Message}", ex.Message);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                null,
                400
            ));
        }
        catch (Exception ex)
        {
            // Error interno del servidor - devolver 500
            _logger.LogError(ex, "Unexpected error in GetAllParametros");
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
        finally
        {
            AuthorizationService.ClearCurrentRequestHeaders();
        }
    }
}
