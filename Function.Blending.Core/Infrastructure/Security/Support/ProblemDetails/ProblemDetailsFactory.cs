using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Function.Blending.Upload.Shared.Json;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Core.Infrastructure.Security.Support.ProblemDetails;

/// <summary>
/// Implementaci�n de RFC 7807 "application/problem+json" con soporte de extensiones.
/// Mantiene compatibilidad con tu formato previo (propiedad top-level "traceId") y a�ade
/// la posibilidad de incluir datos extra en "extensions".
/// </summary>
public sealed class ProblemDetailsFactory
{
  /// <summary>
  /// Modelo RFC 7807 con extensiones.
  /// Nota: mantenemos "traceId" en top-level por compatibilidad con clientes ya existentes.
  /// </summary>
  private sealed class Rfc7807Problem
  {
    [JsonPropertyName("type")] public string Type { get; init; } = "about:blank";
    [JsonPropertyName("title")] public string Title { get; init; } = "";
    [JsonPropertyName("status")] public int Status { get; init; }
    [JsonPropertyName("detail")] public string? Detail { get; init; }
    [JsonPropertyName("instance")] public string? Instance { get; init; }

    // Compatibilidad hacia atr�s: muchos clientes tuyos ya esperan "traceId" en top-level.
    [JsonPropertyName("traceId")] public string? TraceId { get; init; }

    // �Extensiones RFC 7807! Aqu� puedes poner cualquier dato adicional.
    // Ej: requestedBy, scopeRequired, errors con estructura propia, etc.
    [JsonExtensionData] public Dictionary<string, object?> Extensions { get; set; } = new();
  }

  /// <summary>
  /// Escribe un problem+json. "errors" acepta cualquier tipo (puede ser IDictionary&lt;string,string[]&gt;,
  /// o tu propio objeto con validaciones). "extensions" agrega campos libres al payload.
  /// </summary>
  public async Task WriteAsync(
    HttpResponseData res,
    int status,
    string title,
    string type,
    string? detail = null,
    string? traceId = null,
    object? errors = null,
    IDictionary<string, object?>? extensions = null,
    string? instance = null)
  {
    res.StatusCode = (HttpStatusCode)status;
    AddContentTypeIfMissing(res, "application/problem+json");

    var problem = new Rfc7807Problem
    {
      Type = string.IsNullOrWhiteSpace(type) ? "about:blank" : type,
      Title = title,
      Status = status,
      Detail = detail,
      Instance = instance,
      TraceId = traceId
    };

    // 1) Si te pasan "errors", lo exponemos como extensi�n (RFC 7807 lo permite).
    if (errors is not null)
      problem.Extensions["errors"] = errors;

    // 2) Si te pasan extensiones arbitrarias, las fusionamos.
    if (extensions is not null)
    {
      foreach (var kv in extensions)
      {
        // No pisamos campos est�ndar; s�lo agregamos extras.
        if (!problem.Extensions.ContainsKey(kv.Key))
          problem.Extensions[kv.Key] = kv.Value;
      }
    }

    await res.WriteStringAsync(JsonSerializer.Serialize(problem, JsonDefaults.Web));
  }

  // Sobrecarga por compatibilidad (tu c�digo anterior esperaba IDictionary<string,string[]> en "errors").
  public Task WriteAsync(
    HttpResponseData res,
    int status,
    string title,
    string type,
    string? detail,
    string? traceId,
    IDictionary<string, string[]>? errors)
    => WriteAsync(res, status, title, type, detail, traceId, (object?)errors, extensions: null, instance: null);

  private static void AddContentTypeIfMissing(HttpResponseData res, string contentType)
  {
    if (!res.Headers.TryGetValues("Content-Type", out _))
      res.Headers.Add("Content-Type", contentType);
  }
}

public static class ProblemDetailsServiceCollectionExtensions
{
  public static IServiceCollection AddProblemDetailsFactory(this IServiceCollection services)
    => services.AddSingleton<ProblemDetailsFactory>();
}
