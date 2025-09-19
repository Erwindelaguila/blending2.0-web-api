using System.Net;
using System.Text.Json;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.Logging;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Extensions; // ctx.GetCorrelationId()
using Function.Blending.Opt.Infrastructure.Configuration.Options.Logging;
using Function.Blending.Opt.Shared.Extensions;             // ClaimsPrincipal.GetUserId()
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Opt.Functions.Support.ProblemDetails;

public sealed class ProblemDetailsWriter(
  ProblemDetailsFactory factory,
  ISysLogService syslog,
  IRequestContext reqCtx,
  IFunctionContextAccessor fx,
  SysLogOptions opt,
  ILogger<ProblemDetailsWriter> logger) : IProblemDetailsWriter
{
  public async Task<HttpResponseData> CreateAsync(
    FunctionContext ctx,
    HttpRequestData req,
    HttpStatusCode status,
    string type,
    string title,
    string? detail = null,
    object? errors = null,
    IDictionary<string, object?>? extensions = null)
  {
    var res = req.CreateResponse(status);

    await factory.WriteAsync(
      res,
      status: (int)status,
      title: title,
      type: type,
      detail: detail,
      traceId: ctx.GetCorrelationId(),
      errors: errors,
      extensions: extensions,
      instance: req.Url?.PathAndQuery
    );

    await PersistIfEnabledAsync(ctx, req, (int)status, type, title, detail);
    return res;
  }

  public async Task WriteAndSetInvocationResultAsync(
    FunctionContext ctx,
    HttpRequestData req,
    HttpStatusCode status,
    string type,
    string title,
    string? detail = null,
    object? errors = null,
    IDictionary<string, object?>? extensions = null)
  {
    var res = await CreateAsync(ctx, req, status, type, title, detail, errors, extensions);
    ctx.GetInvocationResult().Value = res;
  }

  private async Task PersistIfEnabledAsync(
    FunctionContext ctx,
    HttpRequestData req,
    int status,
    string type,
    string title,
    string? detail)
  {
    if (!opt.Enabled) return;

    try
    {
      var level = status >= 500
        ? SysLogLevel.Error
        : SysLogOptions.MapLevel(opt.Problem4xxAs, SysLogLevel.Warning);

      // Username/UserId
      var username = reqCtx.Username;
      Guid? userId = null;
      var uid = reqCtx.User?.GetUserId();
      if (!string.IsNullOrWhiteSpace(uid) && Guid.TryParse(uid, out var gUid))
        userId = gUid;

      // RequestInvocationId ← correlationId si es GUID
      Guid? requestId = null;
      var corr = ctx.GetCorrelationId();
      if (!string.IsNullOrWhiteSpace(corr) && Guid.TryParse(corr, out var gCorr))
        requestId = gCorr;

      // FunctionInvocationId ← InvocationId si es GUID
      Guid? functionId = null;
      var inv = fx.Current?.InvocationId ?? ctx.InvocationId;
      if (!string.IsNullOrWhiteSpace(inv) && Guid.TryParse(inv, out var gInv))
        functionId = gInv;

      // ExceptionGroupId (si ya viene del pipeline por una excepción previa)
      Guid? groupId = null;
      if (ctx.Items.TryGetValue("ExceptionGroupId", out var eg) && Guid.TryParse(eg?.ToString(), out var gEg))
        groupId = gEg;

      var path = req.Url?.PathAndQuery ?? ctx.FunctionDefinition.Name;
      var message = $"ProblemDetails {status} {type} - {title} @ {path}";

      string? extra = null;
      try
      {
        extra = JsonSerializer.Serialize(new { type, title, detail, path, status });
      }
      catch { /* si falla serializar, seguimos sin extra */ }

      var record = new SysLogRecord(
        Id: Guid.NewGuid(),
        NameSpace: typeof(ProblemDetailsWriter).Namespace ?? "Function.Blending.Opt.Functions.Support.ProblemDetails",
        ClassName: nameof(ProblemDetailsWriter),
        MethodName: nameof(PersistIfEnabledAsync),
        Username: username,
        UserId: userId,
        Message: message,
        StackTrace: null,
        ExtraInfo: extra,
        RequestInvocationId: requestId,
        FunctionInvocationId: functionId,
        ExceptionGroupId: groupId,
        Level: level
      );

      await syslog.WriteAsync(record, CancellationToken.None);
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
      logger.LogWarning(ex, "Non-critical: failed to persist ProblemDetails in SysLog.");
    }
  }
}
