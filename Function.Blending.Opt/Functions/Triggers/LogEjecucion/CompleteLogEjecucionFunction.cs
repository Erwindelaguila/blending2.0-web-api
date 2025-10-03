using Azure;
using Function.Blending.Opt.Application.Features.LogEjecucion.Commands.Complete;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Extensions;
using Function.Blending.Opt.Functions.Support.Http;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Functions.Support.Routing;
using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Results; // ResultKind
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Function.Blending.Opt.Functions.Triggers.LogEjecucion
{
  /// <summary>Webhook superficial de Logística (mismo patrón que Calidad).</summary>
  public sealed class CompleteLogEjecucionFunction(
    IMediator mediator,
    IProblemDetailsWriter problem,
    IRequestContext ctx,
    ISysParamService sysParamService,
    IConfiguration configuration,
    IFunctionContextAccessor fctxAccessor,
    ISysLogService syslog
  )
  {
    [Function(nameof(CompleteLogEjecucionFunction))]
    [AllowAnonymous]
    [Webhook]
    [ValidateHmac(HmacKeys.XSignatureHeaderKey)]
    public async Task<HttpResponseData> Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FunctionRoutes.Logistics.Webhook)]
      HttpRequestData req,
      FunctionContext fctx)
    {
      // A) Lectura robusta: usa RawBody del pipeline si existe; si no, lee el stream
      var body = await req.TryReadJsonWithSysLogAsync<CompleteLogEjecucionRequest>(fctx, ctx, fctxAccessor, syslog);
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

      // B) Usuario del sistema desde SysParam (igual que Calidad; no dependemos de oid/sub)
      var sysParamKey = configuration[ConfigurationKeys.SysParam.SystemUser] ?? SysParamDefaultKeys.SystemUser;
      Guid userId;
      try
      {
        userId = await sysParamService.GetRequiredIdAsync(sysParamKey, fctx.CancellationToken);
      }
      catch(Exception ex)
      {
        return await problem.BadRequestWithLogAsync(
          fctx, req, syslog, ctx, fctxAccessor,
          type: "urn:blending:error:invalid-user",
          title: "Invalid user id",
          detail: $"System user not configured or inactive for SysParam key '{sysParamKey}'.",
          ex: ex,
          extraInfo: $"sysParamKey={sysParamKey}",
          extensions: new Dictionary<string, object?> { ["requestedBy"] = ctx.Username },
          sourceType: typeof(CompleteLogEjecucionFunction),
          methodName: nameof(Run));
      }

      // C) Ejecutar comando (el handler enriquece Estado antes de mapear)
      var cmd = new CompleteLogEjecucionCommand(
        body.Id,
        body.EstadoId,
        userId,
        body.Mensaje
      );

      var result = await mediator.Send(cmd);

      if (!result.IsSuccess)
      {
        var isNotFound = result is Result r && r.Kind == ResultKind.NotFound;
        var (status, type, title, detail) = isNotFound
          ? (HttpStatusCode.NotFound, "urn:blending:error:logistica:not-found", "Not Found", result.Error ?? "Execution not found")
          : (HttpStatusCode.BadRequest, "urn:blending:error:logistica:bad-request", "Bad Request", result.Error ?? "Invalid request");

        return await problem.CreateAsync(
          fctx, req, status, type, title, detail,
          extensions: new Dictionary<string, object?>
          {
            ["requestedBy"] = ctx.Username,
            ["executionId"] = body.Id
          }
        );
      }

      // D) 200 OK con el DTO estándar (mismo helper que en Calidad)
      return await req.OkAsync(result.Value!);
    }
  }
}
