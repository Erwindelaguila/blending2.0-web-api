using System.Net;
using Function.Blending.Core.Infrastructure.Security.Support.ProblemDetails;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Pipeline;

public sealed class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
{
  private readonly ILogger<ExceptionHandlingMiddleware> _logger;
  private readonly ProblemDetailsFactory _pdf;

  public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, ProblemDetailsFactory pdf)
    => (_logger, _pdf) = (logger, pdf);

  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    try
    {
      await next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled exception in pipeline.");

      var req = await context.GetHttpRequestDataAsync();
      if (req is null) throw;

      var res = req.CreateResponse(HttpStatusCode.InternalServerError);
      await _pdf.WriteAsync(
        res,
        status: 500,
        title: "Unexpected error",
        type: "urn:blending:error:unexpected",
        detail: "An unexpected error occurred.",
        traceId: context.Items.TryGetValue("CorrelationId", out var v) ? v?.ToString() : null,
        instance: req.Url.PathAndQuery
      );

      context.GetInvocationResult().Value = res;
    }
  }
}
