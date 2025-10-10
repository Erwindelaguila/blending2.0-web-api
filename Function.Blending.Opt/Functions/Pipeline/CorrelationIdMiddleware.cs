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
  // Encabezados candidatos desde los que intentar leer el ID (respetando tus constantes/estilo)
  private static readonly string[] HeaderCandidates =
  [
    CorrelationKeys.CorrelationHeaderKey,
    "X-Correlation-ID",
    "x-request-id",
    "X-Request-ID"
  ];

  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    // 1) Intentar leer la request (solo existirá en HTTP trigger)
    var req = await context.GetHttpRequestDataAsync();

    // 2) Resolver o crear un CorrelationId
    var correlationId = ResolveCorrelationId(req);
    var traceparent = req?.Headers.TryGetValues(CorrelationKeys.TraceParentHeaderKey, out var tpValues) == true ? tpValues.FirstOrDefault() : null;

    if (string.IsNullOrWhiteSpace(correlationId) && string.IsNullOrWhiteSpace(traceparent))
    { 
      correlationId = Guid.NewGuid().ToString("N");
      traceparent = $"xx-{correlationId}-xx";
    }

    // 3) Publicar en Items para el resto del pipeline
    context.Items[CorrelationKeys.CorrelationIdItemKey] = correlationId;
    context.Items[CorrelationKeys.TraceParentIdItemKey] = traceparent;

    // 4) Log no-crítico (si falla, no debe romper el pipeline)
    try
    {
      logger.LogDebug("CorrelationId assigned: {CorrelationId} and {traceparent}", correlationId, traceparent);
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
      logger.LogWarning(ex, "Non-critical: failed to log correlation id. corr={CorrelationId}, traceparent={traceparent}", correlationId, traceparent);
    }

    // 5) Continuar la ejecución
    await next(context);

    // 6) Adjuntar header a la respuesta HTTP (no crítico)
    try
    {
      if (context.GetInvocationResult().Value is HttpResponseData res)
      {
        if (!res.Headers.TryGetValues(CorrelationKeys.CorrelationHeaderKey, out _))
        {
          res.Headers.Add(CorrelationKeys.CorrelationHeaderKey, correlationId);
        }
        if (!res.Headers.TryGetValues(CorrelationKeys.TraceParentHeaderKey, out _))
        {
          res.Headers.Add(CorrelationKeys.TraceParentHeaderKey, traceparent);
        }
      }
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
      logger.LogWarning(ex, "Non-critical: failed to append correlation header. corr={CorrelationId}, traceparent={traceparent}", correlationId, traceparent);
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
