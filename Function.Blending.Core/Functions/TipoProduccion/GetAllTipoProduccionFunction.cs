using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Net;

namespace Function.Blending.Core.Functions.TipoProduccion;

public class GetAllTipoProduccionFunction
{
    private readonly ILogger<GetAllTipoProduccionFunction> _logger;
    private readonly IMediator _mediator;

    public GetAllTipoProduccionFunction(
        ILogger<GetAllTipoProduccionFunction> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Function(FunctionNames.TipoProduccion.GetAll)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, HttpMethods.Get, Route = ApiRoutes.Core.Production.TipoProduccionBase)] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("GetAllTipoProduccionFunction procesando...");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

            // Obtener parámetros de paginación con valores por defecto
            if (!int.TryParse(query["page"], out var page) || page < 1)
                page = 1;
                
            if (!int.TryParse(query["size"], out var size) || size < 1 || size > 100)
                size = 10;

            var filters = QueryParameterHelper.ParseTipoProduccionFilters(query);
            
            // Solo aplicar filtros si hay filtros activos
            var filtersToApply = QueryParameterHelper.HasActiveFilters(filters) ? filters : null;

            var getAllQuery = new GetAllTipoProduccionWithPaginationQuery(page, size, filtersToApply);

            var result = await _mediator.Send(getAllQuery);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetAllTipoProduccionFunction: {Message}", ex.Message);
            
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "Error interno del servidor" });
            return errorResponse;
        }
    }
}
