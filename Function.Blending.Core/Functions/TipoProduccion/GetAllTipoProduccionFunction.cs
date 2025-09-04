using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.TipoProduccion.Queries;
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
/// Función para obtener todos los tipos de producción con paginación
/// Implementa patrón clean code con manejo de errores estandarizado
/// </summary>
public class GetAllTipoProduccionFunction
{
    private readonly ILogger<GetAllTipoProduccionFunction> _logger;
    private readonly IMediator _mediator;

    public GetAllTipoProduccionFunction(
        ILogger<GetAllTipoProduccionFunction> logger,
        IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [Function(FunctionNames.TipoProduccion.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.TipoProduccionBase)] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("GetAllTipoProduccionFunction procesando...");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

            // Verificar si es solicitud de activos (para combos)
            if (query["activo"] == "true")
            {
                _logger.LogInformation("Returning active tipo produccion for combo");
                var activasResult = await _mediator.Send(new GetAllTipoProduccionActivasQuery());
                return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                    BaseResponse<object>.Success(activasResult, "Tipos de producción activos obtenidos correctamente"));
            }

            // Obtener parámetros de paginación con valores por defecto
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
                
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10;

            var filters = QueryParameterHelper.ParseTipoProduccionFilters(query);
            
            // Solo aplicar filtros si hay filtros activos
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var getAllQuery = new GetAllTipoProduccionQuery(page, size, filtersToApply);

            var result = await _mediator.Send(getAllQuery);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<object>.Success(result, "Tipos de producción obtenidos correctamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetAllTipoProduccionFunction: {Message}", ex.Message);
            
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
