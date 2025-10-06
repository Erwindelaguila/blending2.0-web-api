using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Functions.Pipeline;

public sealed class ExceptionHandlingMiddleware(
  ILogger<ExceptionHandlingMiddleware> logger) : IFunctionsWorkerMiddleware
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
      
      // Re-throw to let Azure Functions handle the error response
      throw;
    }
  }
}
