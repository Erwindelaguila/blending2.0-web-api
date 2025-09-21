using System.Net;
using Function.Blending.Core.Infrastructure.Security.Support.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Infrastructure.Security.Support.ProblemDetails;

public sealed class ProblemDetailsWriter(ProblemDetailsFactory factory) : IProblemDetailsWriter
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
      traceId: ctx.GetCorrelationId(),          // ← siempre
      errors: errors,
      extensions: extensions,
      instance: req.Url?.PathAndQuery           // ← mínimo-útil, centralizado
    );
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
}
