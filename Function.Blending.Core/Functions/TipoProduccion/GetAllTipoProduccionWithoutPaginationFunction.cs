using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Net;

namespace Function.Blending.Core.Functions.TipoProduccion;

public class GetAllTipoProduccionWithoutPaginationFunction
{
    private readonly ILogger<GetAllTipoProduccionWithoutPaginationFunction> _logger;
    private readonly IMediator _mediator;

    public GetAllTipoProduccionWithoutPaginationFunction(
        ILogger<GetAllTipoProduccionWithoutPaginationFunction> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
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

            var getAllQuery = new GetAllTipoProduccionWithoutPaginationQuery(filtersToApply);

            var result = await _mediator.Send(getAllQuery);
            
            return await HttpResponseHelper.WriteBaseResponseAsync(req, 
                BaseResponse<List<TipoProduccionDTO>>.Success(result, "Tipos de producción obtenidos correctamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetAllTipoProduccionWithoutPaginationFunction: {Message}", ex.Message);
            
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "Error interno del servidor" });
            return errorResponse;
        }
    }
}
