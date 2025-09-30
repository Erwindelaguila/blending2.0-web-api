using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Functions.Support.ProblemDetails;

public interface IProblemDetailsWriter
{
  // Para usar en TRIGGERS: crea el HttpResponseData con problem+json (no toca InvocationResult)
  Task<HttpResponseData> CreateAsync(
    FunctionContext ctx,
    HttpRequestData req,
    HttpStatusCode status,
    string type,
    string title,
    string? detail = null,
    object? errors = null,
    IDictionary<string, object?>? extensions = null);

  // Para usar en MIDDLEWARES: escribe problem+json y setea InvocationResult
  Task WriteAndSetInvocationResultAsync(
    FunctionContext ctx,
    HttpRequestData req,
    HttpStatusCode status,
    string type,
    string title,
    string? detail = null,
    object? errors = null,
    IDictionary<string, object?>? extensions = null);
}
