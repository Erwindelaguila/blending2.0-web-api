using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Functions.Support.Authorization;
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

    public GetAllLineasProduccionFunction(
        IMediator mediator, 
        ILogger<GetAllLineasProduccionFunction> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [RequireScopes("Administrador,Logistica")]
    [Function(FunctionNames.LineaProduccion.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Get, Route = ApiRoutes.Core.Production.LineaProduccionBase)] HttpRequestData req)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // Timeout de 30 segundos
        
        try
        {
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            
            _logger.LogInformation("GetAllLineasProduccion called with query: {QueryString}", req.Url.Query);
            
            // Verificar si es solicitud de activos (para combos)
            if (query["activo"] == "true")
            {
                _logger.LogInformation("Returning active lineas produccion for combo");
                var activasResult = await _mediator.Send(new GetAllLineasProduccionActivasQuery(), cts.Token);
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(activasResult, "Líneas de producción activas obtenidas correctamente"));
            }
            
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
                
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10;

            var filters = QueryParameterHelper.ParseLineaProduccionFilters(query);
            
            _logger.LogInformation("Parsed filters - Codigo: {Codigo}, Estado: {Estado}, FechaDesde: {FechaDesde}", 
                filters.Codigo, filters.Estado, filters.FechaDesde);
            
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var result = await _mediator.Send(new GetAllLineasProduccionQuery(page, size, filtersToApply), cts.Token);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<PagedResponse<LineaProduccionDTO>>.Success(result, "Líneas de producción obtenidas correctamente"));
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetAllLineasProduccion request timeout after 30 seconds");
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                "La operación tardó demasiado tiempo y fue cancelada",
                null,
                408
            ));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error in GetAllLineasProduccion: {Message}", ex.Message);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                null,
                400
            ));
        }
        catch (Exception ex)
        {
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
