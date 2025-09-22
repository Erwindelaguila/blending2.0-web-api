using System.Net;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.Logging;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Infrastructure.Configuration.Options.Logging;
using Function.Blending.Opt.Shared.Extensions; // GetUserId()
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Opt.Functions.Pipeline;

public sealed class ExceptionHandlingMiddleware(
  ILogger<ExceptionHandlingMiddleware> logger,
  ProblemDetailsFactory pdf,
  ISysLogService syslog,
  IRequestContext requestContext,
  SysLogOptions opt) : IFunctionsWorkerMiddleware
{
  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    try
    {
      await next(context);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Unhandled exception in function {Function}", context.FunctionDefinition?.Name);

      // Persistir a SysLog si está habilitado
      if (opt.Enabled)
      {
        try
        {
          // Parsear Namespace / Class / Method desde EntryPoint: Namespace.Class.Method
          var entryPoint = context.FunctionDefinition?.EntryPoint;
          string? ns = null, cls = null, method = null;
          if (!string.IsNullOrWhiteSpace(entryPoint))
          {
            var parts = entryPoint.Split('.');
            if (parts.Length >= 2)
            {
              method = parts[^1];
              cls = parts[^2];
              if (parts.Length > 2) ns = string.Join('.', parts, 0, parts.Length - 2);
            }
          }

          // CorrelationId -> RequestInvocationId (si es GUID)
          Guid? requestId = null;
          var corr = requestContext.CorrelationId ?? (context.Items.TryGetValue("CorrelationId", out var c) ? c?.ToString() : null);
          if (!string.IsNullOrWhiteSpace(corr) && Guid.TryParse(corr, out var corrGuid))
            requestId = corrGuid;

          // FunctionInvocationId (si es GUID)
          Guid? functionId = null;
          var inv = context.InvocationId;
          if (!string.IsNullOrWhiteSpace(inv) && Guid.TryParse(inv, out var invGuid))
            functionId = invGuid;

          // ExceptionGroupId: reusar si existe; si no, crear y guardar para la request
          Guid? groupId = null;
          if (context.Items.TryGetValue("ExceptionGroupId", out var egObj) && Guid.TryParse(egObj?.ToString(), out var egGuid))
          {
            groupId = egGuid;
          }
          else
          {
            groupId = Guid.NewGuid();
            context.Items["ExceptionGroupId"] = groupId;
          }

          // Username / UserId
          var username = requestContext.Username;
          Guid? userId = null;
          var uidStr = requestContext.User?.GetUserId();
          if (!string.IsNullOrWhiteSpace(uidStr) && Guid.TryParse(uidStr, out var uidGuid))
            userId = uidGuid;

          var record = new SysLogRecord(
            Id: Guid.NewGuid(),
            NameSpace: ns ?? (context.FunctionDefinition?.Name ?? "Function.Blending.Opt"),
            ClassName: cls,
            MethodName: method,
            Username: username,
            UserId: userId,
            Message: ex.Message,
            StackTrace: ex.ToString(),
            ExtraInfo: null,
            RequestInvocationId: requestId,
            FunctionInvocationId: functionId,
            ExceptionGroupId: groupId,
            Level: SysLogLevel.Error
          );

          await syslog.WriteAsync(record, CancellationToken.None);
        }
        catch (Exception logEx)
        {
          logger.LogWarning(logEx, "Non-critical: failed to write SysLog for unhandled exception.");
        }
      }

      // ProblemDetails al cliente (si es HTTP)
      var req = await context.GetHttpRequestDataAsync();
      if (req is not null)
      {
        var res = req.CreateResponse(HttpStatusCode.InternalServerError);
        await pdf.WriteAsync(
          res,
          status: 500,
          title: "Unexpected error",
          type: "urn:blending:error:unexpected",
          detail: "An unexpected error occurred.",
          traceId: requestContext.CorrelationId ?? (context.Items.TryGetValue("CorrelationId", out var v) ? v?.ToString() : null),
          instance: req.Url.PathAndQuery
        );
        context.GetInvocationResult().Value = res;
      }
    }
  }
}
