using Function.Blending.Opt.Application.Features.CalEjecucion.Commands.Complete;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Http;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Functions.Support.Routing;
using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Results; // Para ResultKind
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Function.Blending.Opt.Functions.Triggers.CalEjecucion;

public sealed class CompleteCalEjecucionFunction(
  IMediator mediator,
  IProblemDetailsWriter problem,
  IRequestContext ctx,
  ISysParamService sysParamService,
  IConfiguration configuration)
{
  [Function(nameof(CompleteCalEjecucionFunction))]
  [AllowAnonymous]
  [Webhook]
  [ValidateHmac(HmacKeys.XSignatureHeaderKey)]
  public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FunctionRoutes.Quality.Webhook)]
    HttpRequestData req,
    FunctionContext fctx)
  {
    // B) Lectura robusta: usa RawBody del pipeline si existe; si no, lee el stream
    var body = await req.TryReadFromJsonOrRawAsync<CompleteCalEjecucionRequest>(fctx);
    if (body is null)
    {
      return await problem.CreateAsync(
        fctx, req, HttpStatusCode.BadRequest,
        type: "urn:blending:error:invalid-payload",
        title: "Bad Request",
        detail: "Invalid JSON body",
        extensions: new Dictionary<string, object?> { ["requestedBy"] = ctx.Username }
      );
    }

    // === Usuario del sistema desde SysParam (sin depender de principal/oid) ===
    var sysParamKey = configuration[ConfigurationKeys.SysParam.SystemUser] ?? SysParamDefaultKeys.SystemUser;
    Guid userId;
    try
    {
      userId = await sysParamService.GetRequiredIdAsync(sysParamKey, fctx.CancellationToken);
    }
    catch
    {
      return await problem.CreateAsync(
        fctx, req, HttpStatusCode.BadRequest,
        type: "urn:blending:error:invalid-user",
        title: "Invalid user id",
        detail: $"System user not configured or inactive for SysParam key '{sysParamKey}'.");
    }

    var cmd = new CompleteCalEjecucionCommand(
      body.Id,
      body.EstadoId,
      body.Mensaje,
      userId,
      body.Resumenes,
      body.Detalles
    );

    var result = await mediator.Send(cmd);

    if (!result.IsSuccess)
    {
      var isNotFound = result is Result r && r.Kind == ResultKind.NotFound;
      var (status, type, title, detail) = isNotFound
        ? (HttpStatusCode.NotFound, "urn:blending:error:calidad:not-found", "Not Found", result.Error ?? "Execution not found")
        : (HttpStatusCode.BadRequest, "urn:blending:error:calidad:bad-request", "Bad Request", result.Error ?? "Invalid request");

      return await problem.CreateAsync(
        fctx, req, status, type, title, detail,
        extensions: new Dictionary<string, object?>
        {
          ["requestedBy"] = ctx.Username,
          ["executionId"] = body.Id
        }
      );
    }

    // 200 OK con el DTO estándar
    return await req.OkAsync(result.Value!);
  }
}
