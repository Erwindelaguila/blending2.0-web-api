using Function.Blending.Core.Infrastructure.Security.Support.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Pipeline;

/// <summary>
/// Middleware que asegura la presencia de un CorrelationId por invocación:
/// - Lo intenta leer de headers comunes (x-correlation-id, x-request-id, traceparent).
/// - Si no existe, genera uno (Guid "N").
/// - Lo coloca en FunctionContext.Items["CorrelationId"] para que lo consuman el resto de componentes.
/// - Opcionalmente, si la respuesta es HTTP, adjunta el header "x-correlation-id".
/// </summary>
public sealed class CorrelationIdMiddleware(ILogger<CorrelationIdMiddleware> logger) : IFunctionsWorkerMiddleware
{
  private const string CorrelationIdItemKey = CorrelationKeys.CorrelationIdItemKey;
  private const string CorrelationHeaderKey = CorrelationKeys.CorrelationHeaderKey;

  // Encabezados candidatos desde los que intentar leer el ID:
  private static readonly string[] HeaderCandidates = new[]
  {
        CorrelationHeaderKey,
        "X-Correlation-ID",
        "x-request-id",
        "X-Request-ID",
        "traceparent" // si viene W3C traceparent, lo usamos tal cual
    };

  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    // 1) Intentar leer la request (solo existirá en HTTP trigger)
    var req = await context.GetHttpRequestDataAsync();

    // 2) Resolver o crear un CorrelationId
    var correlationId = ResolveCorrelationId(req);
    if (string.IsNullOrWhiteSpace(correlationId))
    {
      correlationId = Guid.NewGuid().ToString("N");
    }

    // 3) Guardarlo en FunctionContext.Items para que otros middlewares/Functions lo consuman
    context.Items[CorrelationIdItemKey] = correlationId;

    // 4) Log (no debe romper el pipeline si falla)
    try
    {
      logger.LogDebug("CorrelationId assigned: {CorrelationId}", correlationId);
    }
    catch
    {
      // nunca interrumpir el pipeline por logging
    }

    // 5) Continuar la ejecución
    await next(context);

    // 6) Si hay respuesta HTTP, adjunta el header x-correlation-id (con seguridad)
    try
    {
      if (context.GetInvocationResult().Value is HttpResponseData res)
      {
        if (!res.Headers.TryGetValues(CorrelationHeaderKey, out _))
        {
          res.Headers.Add(CorrelationHeaderKey, correlationId);
        }
      }
    }
    catch
    {
      // nunca interrumpir el pipeline por adjuntar un header
    }
  }

  private static string? ResolveCorrelationId(HttpRequestData? req)
  {
    if (req is null) return null;

    foreach (var key in HeaderCandidates)
    {
      if (req.Headers.TryGetValues(key, out var values))
      {
        var first = values.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(first))
          return first.Trim();
      }
    }
    return null;
  }
}
