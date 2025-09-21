using System.Net;
using Function.Blending.Core.Infrastructure.Security.Support.Http;
using Function.Blending.Core.Infrastructure.Security.Support.ProblemDetails;
using Function.Blending.Upload.Functions.Configuration.Options;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;

namespace Function.Blending.Upload.Functions.Pipeline;

public sealed class RequestSizeLimitMiddleware(IOptions<RequestSizeOptions> opts, ProblemDetailsFactory pdf)
  : IFunctionsWorkerMiddleware
{
  private readonly RequestSizeOptions _opts = opts.Value;
  private readonly ProblemDetailsFactory _pdf = pdf;

  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    var req = await context.GetHttpRequestDataAsync();
    if (req is null) { await next(context); return; }

    var method = req.Method?.ToUpperInvariant();
    if (method is "POST" or "PUT" or "PATCH")
    {
      if (req.Headers.TryGetValues("Content-Length", out var vals))
      {
        var raw = vals.FirstOrDefault();
        if (long.TryParse(raw, out var len) && len > _opts.MaxBytes)
        {
          var res = req.CreateResponse(HttpStatusCode.RequestEntityTooLarge);
          var traceId = context.Items.TryGetValue(CorrelationKeys.CorrelationIdItemKey, out var v) ? v?.ToString() : null;

          await _pdf.WriteAsync(res,
            status: 413,
            title: "Payload Too Large",
            type: "urn:blending:error:payload-too-large",
            detail: $"Payload too large. Limit={_opts.MaxBytes} bytes.",
            traceId: traceId);

          context.GetInvocationResult().Value = res;
          return;
        }
      }
    }

    await next(context);
  }
}
