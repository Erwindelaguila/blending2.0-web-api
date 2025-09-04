using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Web;
using System.Net;

namespace Function.Blending.Core.Functions.Agregado;

public class GetAllAgregadosFunction
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetAllAgregadosFunction> _logger;

    public GetAllAgregadosFunction(
        IMediator mediator, 
        ILogger<GetAllAgregadosFunction> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function(FunctionNames.Agregado.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.AgregadoBase)] HttpRequestData req)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        
        try
        {
            
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            
            _logger.LogInformation("GetAllAgregados called with query: {QueryString}", req.Url.Query);
            
            if (query["activo"] == "true")
            {
                _logger.LogInformation("Returning active agregados for combo");
                var activosResult = await _mediator.Send(new GetAllAgregadosActivosQuery(req), cts.Token);
                return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Success(activosResult, "Agregados activos obtenidos correctamente"));
            }
            
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
                
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10;

            var filters = QueryParameterHelper.ParseAgregadoFilters(query);
            
            _logger.LogInformation("Parsed filters - FechaDesde: {Desde}", filters.FechaDesde);
            
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var result = await _mediator.Send(new GetAllAgregadosQuery(page, size, filtersToApply, req), cts.Token);
            
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<PagedResponse<AgregadoDTO>>.Success(result, "Agregados obtenidos correctamente"));
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetAllAgregados request timeout after 30 seconds");
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                "La operación tardó demasiado tiempo y fue cancelada",
                null,
                408
            ));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error in GetAllAgregados: {Message}", ex.Message);
            return await HttpResponseHelper.WriteBaseResponseAsync(req, BaseResponse<object>.Fail(
                ex.Message,
                null,
                400
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetAllAgregados");
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
