using System.Net;
using Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetById;
using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Http;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Functions.Support.Routing;
using Function.Blending.Opt.Shared.Constants;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Opt.Functions.Triggers.CalEjecucion;

public sealed class GetCalEjecucionByIdFunction(
  IMediator mediator,
  IProblemDetailsWriter problem,
  IRequestContext ctx)
{
  [Function(nameof(GetCalEjecucionByIdFunction))]
  [RequireScopes(ConfigurationKeys.Auth.Scopes.Quality.ReadById)]
  public async Task<HttpResponseData> Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = FunctionRoutes.Quality.GetById)]
      HttpRequestData req,
      FunctionContext fctx,  // ← necesitamos el context para el writer
      Guid id)
  {
    var qs = req.GetQuery();

    var expand = qs.GetEntries(
      key: "expand",
      allowed: ["input", "output"],
      synonyms: new Dictionary<string, string> { ["in"] = "input", ["out"] = "output" },
      separators: [',', ';', '|']
    );

    var (convertDates, tzId) = DateConversionRequestOptions.From(req);

    var result = await mediator.Send(new GetCalEjecucionByIdQuery(id, expand, convertDates, tzId));

    if (!result.IsSuccess || result.Value is null || result.Value.Data is null)
    {
      return await problem.CreateAsync(
        fctx,
        req,
        HttpStatusCode.NotFound,
        type: "urn:blending:error:calidad:not-found",
        title: "Not Found",
        detail: "Execution not found",
        extensions: new Dictionary<string, object?>
        {
          ["requestedBy"] = ctx.Username,
          ["executionId"] = id
        });
    }

    var payload = result.Value;
    // data + meta del handler
    return await req.OkAsync(payload.Data, new { conversion = payload.Meta });
  }
}
