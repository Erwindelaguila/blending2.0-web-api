using Function.Blending.Opt.Functions.Support.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Opt.Functions.Pipeline;

/// <summary>
/// Asegura un CorrelationId por invocación:
/// - Intenta leerlo de headers (x-correlation-id, x-request-id, traceparent).
/// - Si no existe, genera uno (Guid "N").
/// - Lo coloca en FunctionContext.Items[CorrelationKeys.CorrelationIdItemKey].
/// - Si la respuesta es HTTP, adjunta el header CorrelationKeys.CorrelationHeaderKey.
/// </summary>
public sealed class CorrelationIdMiddleware(ILogger<CorrelationIdMiddleware> logger) : IFunctionsWorkerMiddleware
{
  private const string CorrelationIdItemKey = CorrelationKeys.CorrelationIdItemKey;
  private const string CorrelationHeaderKey = CorrelationKeys.CorrelationHeaderKey;

  // Encabezados candidatos desde los que intentar leer el ID (respetando tus constantes/estilo)
  private static readonly string[] HeaderCandidates = new[]
  {
    CorrelationHeaderKey,
    "X-Correlation-ID",
    "x-request-id",
    "X-Request-ID",
    "traceparent"
  };

  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    // 1) Intentar leer la request (solo existirá en HTTP trigger)
    var req = await context.GetHttpRequestDataAsync();

    // 2) Resolver o crear un CorrelationId
    var correlationId = ResolveCorrelationId(req);
    if (string.IsNullOrWhiteSpace(correlationId))
      correlationId = Guid.NewGuid().ToString("N");

    // 3) Publicar en Items para el resto del pipeline
    context.Items[CorrelationIdItemKey] = correlationId;

    // 4) Log no-crítico (si falla, no debe romper el pipeline)
    try
    {
      logger.LogDebug("CorrelationId assigned: {CorrelationId}", correlationId);
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
      logger.LogWarning(ex, "Non-critical: failed to log correlation id. corr={CorrelationId}", correlationId);
    }

    // 5) Continuar la ejecución
    await next(context);

    // 6) Adjuntar header a la respuesta HTTP (no crítico)
    try
    {
      if (context.GetInvocationResult().Value is HttpResponseData res && !res.Headers.TryGetValues(CorrelationHeaderKey, out _))
      {
        res.Headers.Add(CorrelationHeaderKey, correlationId);
      }
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
      logger.LogWarning(ex, "Non-critical: failed to append correlation header. corr={CorrelationId}", correlationId);
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
