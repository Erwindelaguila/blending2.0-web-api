using System.Diagnostics;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Functions.Pipeline;

public sealed class RequestLoggingMiddleware(
  ILogger<RequestLoggingMiddleware> logger) : IFunctionsWorkerMiddleware
{
  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    var sw = Stopwatch.StartNew();
    var name = context.FunctionDefinition.Name;
    
    logger.LogInformation("REQ start {Function}", name);

    try
    {
      await next(context);
      sw.Stop();
      
      logger.LogInformation("REQ end {Function} elapsed_ms={Elapsed}", name, sw.ElapsedMilliseconds);
    }
    catch (Exception ex)
    {
      sw.Stop();
      logger.LogError(ex, "REQ error {Function} elapsed_ms={Elapsed}", name, sw.ElapsedMilliseconds);
      throw;
    }
  }
}
