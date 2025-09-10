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
  [RequireScopes(ConfigurationKeys.Auth.Scopes.Quality.ReadHistory)]
  public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = FunctionRoutes.Logistics.History)]
    HttpRequestData req,
    FunctionContext fctx)
  {
    // === Query parseado con helpers reutilizables ===
    var qs = req.GetQuery();

    var page = qs.GetIntOrDefault("page", 1);
    var pageSize = qs.GetIntOrDefault("pageSize", 50);

    var sortBy = qs.GetStringOrNull("sortBy");
    var sortDir = qs.GetStringOrNull("sortDir");

    // Acepta 'creadoDel' o 'creadoDelUtc' (igual para 'creadoAl')
    var creadoDelUtc = qs.GetUtcDateTime("creadoDelUtc", "creadoDel");
    var creadoAlUtc = qs.GetUtcDateTime("creadoAlUtc", "creadoAl");

    Guid? estadoId = qs.GetGuidOrNull("estadoId");
    Guid? plantaId = qs.GetGuidOrNull("plantaId");
    bool? confirmado = qs.GetBoolOrNull("confirmado");

    var codigo = qs.GetStringOrNull("codigo");

    var result = await mediator.Send(
      new GetLogEjecucionHistoryQuery(
        page, pageSize, sortBy, sortDir, confirmado,
        creadoDelUtc, creadoAlUtc, estadoId, plantaId, codigo));

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
            sortDir = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc",
            confirmado,
            creadoDel = creadoDelUtc,
            creadoAl = creadoAlUtc,
            estadoId,
            plantaId,
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
      sortDir = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc",
      requestedBy = ctx.Username,
      filters = new
      {
        creadoDel = creadoDelUtc,
        creadoAl = creadoAlUtc,
        estadoId,
        plantaId,
        confirmado,
        codigo
      }
    };

    return await req.OkAsync(data, meta);
  }
}