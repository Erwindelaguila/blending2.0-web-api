using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Planta.Queries;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Web;

namespace Function.Blending.Core.Functions.Planta;

public class GetAllPlantasFunction
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetAllPlantasFunction> _logger;
    private readonly IAuthorizationHeaderExtractor _headerExtractor;

    public GetAllPlantasFunction(IMediator mediator, ILogger<GetAllPlantasFunction> logger, IAuthorizationHeaderExtractor headerExtractor)
    {
        _mediator = mediator;
        _logger = logger;
        _headerExtractor = headerExtractor;
    }

    [Function(FunctionNames.Planta.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Get, Route = ApiRoutes.Core.Planta.Base)] HttpRequestData req)
    {
        try
        {
            
            var jwtToken = _headerExtractor.ExtractJwtToken(req);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                AuthorizationService.SetCurrentJwtToken(jwtToken);
                AuthorizationService.SetCurrentRequestData(req);
            }

            var query = HttpUtility.ParseQueryString(req.Url.Query);
            
            // Log para debug - ver qué parámetros llegan
            _logger.LogInformation("GetAllPlantas called with query: {QueryString}", req.Url.Query);
            
            var isHarina = query.Get("isHarina") ?? "0";

            if (isHarina != "0" && isHarina != "1")
            {
                throw new ArgumentException("El parámetro 'isHarina' debe ser '0' o '1'.");
            }

            // Aquí puedes convertirlo a bool si quieres
            bool esHarina = isHarina == "1";
            
            // Obtener parámetros de paginación con valores por defecto
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
                
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10; // Por defecto 10 registros por página

            // Parsear filtros desde query parameters - puede lanzar ArgumentException
            var filters = QueryParameterHelper.ParsePlantaFilters(query);
            
            // Log para debug - ver qué filtros se parsearon
            _logger.LogInformation("Parsed filters - Codigo: {Codigo}, Estado: {Estado}, FechaDesde: {FechaDesde}", 
                filters.Codigo, filters.Estado, filters.FechaDesde);
            
            // Solo enviar filtros si al menos uno está activo
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var queryRequest = new GetAllPlantasQuery(page, size, filtersToApply, esHarina, req);
            var result = await _mediator.Send(queryRequest);

            if (esHarina)
            {
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<List<PlantaShortDTO>>.Success(result.PlantaShortList ?? new List<PlantaShortDTO>(), "Plantas obtenidas correctamente"));
            }
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<PagedResponse<PlantaDTO>>.Success(result.PlantaPaginate ?? new PagedResponse<PlantaDTO>(), "Plantas obtenidas correctamente"));
        }
        catch (ArgumentException ex)
        {
            // Error de validación (parámetros inválidos) - devolver 400
            _logger.LogWarning("Validation error in GetAllPlantas: {Message}", ex.Message);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                null,
                400
            ));
        }
        catch (Exception ex)
        {
            // Error interno del servidor - devolver 500
            _logger.LogError(ex, "Unexpected error in GetAllPlantas");
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
            
            AuthorizationService.ClearCurrentContext();
        }
    }
}
