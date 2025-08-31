using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Infrastructure.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Net;

namespace Function.Blending.Core.Functions.TipoProduccion;

/// <summary>
/// Función para obtener todos los tipos de producción sin paginación
/// Implementa patrón clean code con manejo de errores estandarizado
/// </summary>
public class GetAllTipoProduccionWithoutPaginationFunction
{
    private readonly ILogger<GetAllTipoProduccionWithoutPaginationFunction> _logger;
    private readonly IMediator _mediator;

    public GetAllTipoProduccionWithoutPaginationFunction(
        ILogger<GetAllTipoProduccionWithoutPaginationFunction> logger,
        IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Function(FunctionNames.TipoProduccion.GetAllWithoutPagination)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.TipoProduccionBase + "/all")] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("GetAllTipoProduccionWithoutPaginationFunction procesando...");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var filters = QueryParameterHelper.ParseTipoProduccionFilters(query);
            
            // Solo aplicar filtros si hay filtros activos
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var getAllQuery = new GetAllTipoProduccionWithoutPaginationQuery(filtersToApply, new { Filters = filtersToApply });

            var result = await _mediator.Send(getAllQuery);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<List<TipoProduccionDTO>>.Success(result, "Tipos de producción obtenidos correctamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetAllTipoProduccionWithoutPaginationFunction: {Message}", ex.Message);
            
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
