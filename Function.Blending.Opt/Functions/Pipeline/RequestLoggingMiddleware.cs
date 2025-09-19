using System.Diagnostics;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.Logging;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Http;
using Function.Blending.Opt.Infrastructure.Configuration.Options.Logging;
using Function.Blending.Opt.Shared.Extensions; // GetUserId()
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Opt.Functions.Pipeline;

public sealed class RequestLoggingMiddleware(
  ILogger<RequestLoggingMiddleware> logger,
  ISysLogService syslog,
  IRequestContext reqCtx,
  SysLogOptions opt
) : IFunctionsWorkerMiddleware
{
  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    var sw = Stopwatch.StartNew();
    var name = context.FunctionDefinition.Name;
    var corr = context.Items.TryGetValue(CorrelationKeys.CorrelationIdItemKey, out var v) ? v?.ToString() : null;

    logger.LogInformation("REQ start {Function} corr=({CorrelationId})", name, corr);

    HttpRequestData? req = null;
    try
    {
      req = await context.GetHttpRequestDataAsync();

      await next(context);
      sw.Stop();

      int status = 0;
      if (context.GetInvocationResult()?.Value is HttpResponseData http)
        status = (int)http.StatusCode;

      // Persistir Info sólo si está habilitado por config
      if (opt.Enabled && opt.SaveInfo && req is not null)
      {
        try
        {
          // Username / UserId
          var username = reqCtx.Username;
          Guid? userId = null;
          var uid = reqCtx.User?.GetUserId();
          if (!string.IsNullOrWhiteSpace(uid) && Guid.TryParse(uid, out var g))
            userId = g;

          // RequestInvocationId (CorrelationId si es GUID)
          Guid? requestId = null;
          if (!string.IsNullOrWhiteSpace(corr) && Guid.TryParse(corr, out var rid))
            requestId = rid;

          // FunctionInvocationId (si es GUID)
          Guid? functionId = null;
          var inv = context.InvocationId;
          if (!string.IsNullOrWhiteSpace(inv) && Guid.TryParse(inv, out var fid))
            functionId = fid;

          var path = req.Url?.PathAndQuery ?? name;
          var msg = $"REQ end {name} status={status} elapsed_ms={sw.ElapsedMilliseconds} path={path}";

          var record = new SysLogRecord(
            Id: Guid.NewGuid(),
            NameSpace: typeof(RequestLoggingMiddleware).Namespace ?? "Function.Blending.Opt.Functions.Pipeline",
            ClassName: nameof(RequestLoggingMiddleware),
            MethodName: nameof(Invoke),
            Username: username,
            UserId: userId,
            Message: msg,
            StackTrace: null,
            ExtraInfo: null,
            RequestInvocationId: requestId,
            FunctionInvocationId: functionId,
            ExceptionGroupId: context.Items.TryGetValue("ExceptionGroupId", out var eg) && Guid.TryParse(eg?.ToString(), out var gid) ? gid : null,
            Level: SysLogLevel.Info
          );

          await syslog.WriteAsync(record, CancellationToken.None);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
          logger.LogWarning(ex, "Non-critical: failed to persist Info request log. corr={CorrelationId}", corr);
        }
      }

      logger.LogInformation("REQ end   {Function} corr=({CorrelationId}) elapsed_ms={Elapsed} status={Status}",
        name, corr, sw.ElapsedMilliseconds, status);
    }
    catch (Exception ex)
    {
      sw.Stop();
      logger.LogError(ex, "REQ error {Function} corr=({CorrelationId}) elapsed_ms={Elapsed}",
        name, corr, sw.ElapsedMilliseconds);
      throw;
    }
  }
}
