using Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetHistory;
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

namespace Function.Blending.Opt.Functions.Triggers.LogEjecucion;

public sealed class GetLogEjecucionHistoryFunction(
  IMediator mediator,
  IProblemDetailsWriter problem,
  IRequestContext ctx)
{
  [Function(nameof(GetLogEjecucionHistoryFunction))]
  [RequireScopes(ConfigurationKeys.Auth.Scopes.Logistics.ReadHistory)]
  public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = FunctionRoutes.Logistics.History)]
    HttpRequestData req,
    FunctionContext fctx)
  {
    // === Query parseado con helpers reutilizables ===
    var qs = req.GetQuery();

    var page = qs.GetIntOrDefault(CriteriaConstants.Paging.Page, 1);
    var pageSize = qs.GetIntOrDefault(CriteriaConstants.Paging.PageSize, 50);

    var sortBy = qs.GetStringOrNull(CriteriaConstants.Sorting.SortBy);
    var sortDir = qs.GetStringOrNull(CriteriaConstants.Sorting.SortDir);

    // Acepta 'creadoDel' o 'creadoDelUtc' (igual para 'creadoAl')
    var creadoDelUtc = qs.GetUtcDateTime("creadoDelUtc", "creadoDel");
    var creadoAlUtc = qs.GetUtcDateTime("creadoAlUtc", "creadoAl");

    Guid? estadoId = qs.GetGuidOrNull("estadoId");
    bool? confirmado = qs.GetBoolOrNull("confirmado");

    var codigo = qs.GetStringOrNull("codigo");
    var contrato = qs.GetStringOrNull("contrato");

    var result = await mediator.Send(
      new GetLogEjecucionHistoryQuery(page, pageSize, sortBy, sortDir, confirmado, creadoDelUtc, creadoAlUtc, estadoId, codigo, contrato));

    if (!result.IsSuccess)
    {
      return await problem.CreateAsync(
        fctx,
        req,
        HttpStatusCode.NotFound,
        type: "urn:blending:error:logistica:history-not-found",
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
            confirmado,
            creadoDel = creadoDelUtc,
            creadoAl = creadoAlUtc,
            estadoId,
            codigo
          }
        });
    }

    var pr = result.Value!;
    var data = pr.Items;
    var meta = new
    {
      page = pr.Page,
      pageSize = pr.PageSize,
      total = pr.Total,
      totalPages = pr.TotalPages,
      sortBy = sortBy ?? "creadoEl",
      sortDir = string.Equals(sortDir, CriteriaConstants.Sorting.Ascending, StringComparison.OrdinalIgnoreCase) ? CriteriaConstants.Sorting.Ascending : CriteriaConstants.Sorting.Descending,
      requestedBy = ctx.Username,
      filters = new
      {
        creadoDel = creadoDelUtc,
        creadoAl = creadoAlUtc,
        estadoId,
        confirmado,
        codigo
      }
    };

    return await req.OkAsync(data, meta);
  }
}