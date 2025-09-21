using System.Diagnostics;
using Function.Blending.Upload.Functions.Support.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Pipeline;

public sealed class RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger) : IFunctionsWorkerMiddleware
{
  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    var sw = Stopwatch.StartNew();
    var name = context.FunctionDefinition.Name;
    var corr = context.Items.TryGetValue(CorrelationKeys.CorrelationIdItemKey, out var v) ? v?.ToString() : null;

    logger.LogInformation("REQ start {Function} corr=({CorrelationId})", name, corr);

    try
    {
      await next(context);
      sw.Stop();

      int? status = null;
      if (context.GetInvocationResult()?.Value is HttpResponseData http)
        status = (int)http.StatusCode;

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
