using Function.Blending.Opt.Application.Features.CalEjecucion.Commands.ChangeAccepted;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests;
using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Extensions;
using Function.Blending.Opt.Functions.Support.Extensions.BindingContext.BindingData;
using Function.Blending.Opt.Functions.Support.Http;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Functions.Support.Routing;
using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Function.Blending.Opt.Functions.Triggers.CalEjecucion;

public sealed class ChangeAcceptedCalEjecucionFunction(IMediator mediator, IProblemDetailsWriter problem, IRequestContext ctx)
{
  [Function(nameof(ChangeAcceptedCalEjecucionFunction))]
  [RequireScopes(ConfigurationKeys.Auth.Scopes.Quality.ChangeAccepted)]
  public async Task<HttpResponseData> Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FunctionRoutes.Quality.ChangeAccepted)]
      HttpRequestData req,
      FunctionContext fctx)
  {
    var id = fctx.TryGuidBindingData("id");
    if (id is null)
    {
      return await problem.CreateAsync(
        fctx, req, HttpStatusCode.BadRequest,
        type: "urn:blending:error:invalid-route",
        title: "Bad Request",
        detail: "Route parameter 'id' is invalid.",
        extensions: new Dictionary<string, object?> { ["requestedBy"] = ctx.Username });
    }

    var dto = await req.TryReadJsonAsync<ChangeAcceptedCalEjecutionRequest>();
    if (dto is null)
    {
      return await problem.CreateAsync(
        fctx, req, HttpStatusCode.BadRequest,
        type: "urn:blending:error:invalid-payload",
        title: "Bad Request",
        detail: "Invalid JSON body");
    }

    var userIdStr = fctx.GetUserObjectId();
    if (!Guid.TryParse(userIdStr, out var userId))
    {
      return await problem.CreateAsync(
        fctx, req, HttpStatusCode.BadRequest,
        type: "urn:blending:error:invalid-user",
        title: "Invalid user id",
        detail: "The current principal does not provide a valid object id (oid/sub) to audit.");
    }

    var result = await mediator.Send(new ChangeAcceptedCalEjecucionCommand((Guid)id, dto.Grupos, userId));

    if (!result.IsSuccess)
    {
      var isNotFound = result is Result r && r.Kind == ResultKind.NotFound;
      var status = isNotFound ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest;

      return await problem.CreateAsync(
        fctx, req, status,
        type: isNotFound ? "urn:blending:error:quality:not-found" : "urn:blending:error:quality:bad-request",
        title: isNotFound ? "Not Found" : "Bad Request",
        detail: result.Error ?? "Invalid request",
        extensions: new Dictionary<string, object?>
        {
          ["requestedBy"] = ctx.Username,
          ["executionId"] = id
        });
    }

    return await req.OkAsync(result.Value!);
  }
}
