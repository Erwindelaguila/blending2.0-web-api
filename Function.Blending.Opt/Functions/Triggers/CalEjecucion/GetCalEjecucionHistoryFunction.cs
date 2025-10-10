using Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetHistory;
using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Http;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Shared.Constants;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Function.Blending.Opt.Functions.Support.Routing;

namespace Function.Blending.Opt.Functions.Triggers.CalEjecucion;

public sealed class GetCalEjecucionHistoryFunction(
  IMediator mediator,
  IProblemDetailsWriter problem,
  IRequestContext ctx)
{
  [Function(nameof(GetCalEjecucionHistoryFunction))]
  [RequireScopes(ConfigurationKeys.Auth.Scopes.Quality.ReadHistory)]
  public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = FunctionRoutes.Quality.History)]
    HttpRequestData req,
    FunctionContext fctx)
  {
    var qs = req.GetQuery();

    var page = qs.GetIntOrDefault(CriteriaConstants.Paging.Page, 1);
    var pageSize = qs.GetIntOrDefault(CriteriaConstants.Paging.PageSize, 50);

    var sortBy = qs.GetStringOrNull(CriteriaConstants.Sorting.SortBy);
    var sortDir = qs.GetStringOrNull(CriteriaConstants.Sorting.SortDir);

    // Acepta 'creadoDel' o 'creadoDelUtc' (igual para 'creadoAl')
    var creadoDelUtc = qs.GetUtcDateTime("creadoDelUtc", "creadoDel");
    var creadoAlUtc = qs.GetUtcDateTime("creadoAlUtc", "creadoAl");

    Guid? estadoId = qs.GetGuidOrNull("estadoId");
    Guid? plantaId = qs.GetGuidOrNull("plantaId");

    var codigo = qs.GetStringOrNull("codigo");

    var (convertDates, tzId) = DateConversionRequestOptions.From(req);

    var result = await mediator.Send(new GetCalEjecucionHistoryQuery(page, pageSize, sortBy, sortDir, creadoDelUtc, creadoAlUtc, estadoId, plantaId, codigo, convertDates, tzId));

    // Nota: tu handler siempre devuelve Ok(); mantenemos la rama por consistencia
    if (!result.IsSuccess)
    {
      return await problem.CreateAsync(
        fctx,
        req,
        HttpStatusCode.NotFound,
        type: "urn:blending:error:calidad:history-not-found",
        title: "Not Found",
        detail: "History not found",
        extensions: new Dictionary<string, object?>
        {
          ["requestedBy"] = ctx.Username,
          ["filtersEcho"] = new
          {
            page,
            pageSize,
            sortBy = sortBy ?? "creadoEl",
            sortDir = string.Equals(sortDir, CriteriaConstants.Sorting.Ascending, StringComparison.OrdinalIgnoreCase) ? CriteriaConstants.Sorting.Ascending : CriteriaConstants.Sorting.Descending,
            creadoDel = creadoDelUtc,
            creadoAl = creadoAlUtc,
            estadoId,
            plantaId,
            codigo
          }
        });
    }

    var pr = result.Value!;
    var data = pr.Data.Items;
    var meta = new
    {
      page = pr.Data.Page,
      pageSize = pr.Data.PageSize,
      total = pr.Data.Total,
      totalPages = pr.Data.TotalPages,
      sortBy = (sortBy ?? "creadoEl"),
      sortDir = string.Equals(sortDir, CriteriaConstants.Sorting.Ascending, StringComparison.OrdinalIgnoreCase) ? CriteriaConstants.Sorting.Ascending : CriteriaConstants.Sorting.Descending,
      requestedBy = ctx.Username,
      filters = new
      {
        creadoDel = creadoDelUtc,
        creadoAl = creadoAlUtc,
        estadoId,
        plantaId,
        codigo
      },
      conversion = pr.Meta
    };

    return await req.OkAsync(data, meta);
  }
}