using System.Net;
using Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetInputById;
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

public sealed class GetCalEjecucionInputByIdFunction(
  IMediator mediator,
  IProblemDetailsWriter problem,
  IRequestContext ctx)
{
  [Function(nameof(GetCalEjecucionInputByIdFunction))]
  [RequireScopes(ConfigurationKeys.Auth.Scopes.Quality.ReadInputById)]
  public async Task<HttpResponseData> Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = FunctionRoutes.Quality.GetInputById)]
      HttpRequestData req,
      FunctionContext fctx,  // ← necesitamos el context para el writer
      Guid id)
  {
    var result = await mediator.Send(new GetCalEjecucionInputByIdQuery(id));

    if (!result.IsSuccess || result.Value is null)
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

    return await req.OkAsync(result.Value);
  }
}
