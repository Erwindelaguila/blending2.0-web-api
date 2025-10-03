using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Opt.Functions.Support.Http;

public static class RequestJsonExtensions
{
  // Configuración base estricta (sin PropertyNameCaseInsensitive)
  private static readonly JsonSerializerOptions DefaultJson = new(JsonSerializerDefaults.Web);

  public static async Task<T?> ReadFromJsonOrRawAsync<T>(
    this HttpRequestData req,
    FunctionContext ctx,
    CancellationToken cancellationToken = default)
  {
    if (ctx.Items.TryGetValue(HmacKeys.RawBodyItemsKey, out var rawObj) &&
        rawObj is string raw &&
        !string.IsNullOrWhiteSpace(raw))
    {
      // Deserialización manual usando System.Text.Json con opciones estrictas
      return JsonSerializer.Deserialize<T>(raw, DefaultJson);
    }

    // Usa la extensión del Worker directamente
    return await req.ReadFromJsonAsync<T>(cancellationToken);
  }

  /// <summary>
  /// Intenta deserializar el JSON. Si falla, devuelve default y ejecuta onError(ex, raw).
  /// </summary>
  public static async Task<T?> TryReadFromJsonOrRawAsync<T>(
    this HttpRequestData req,
    FunctionContext ctx,
    Func<Exception, string, Task>? onError = null,
    CancellationToken cancellationToken = default)
  {
    string? raw = null;
    try
    {
      if (ctx.Items.TryGetValue(HmacKeys.RawBodyItemsKey, out var rawObj) && rawObj is string s)
        raw = s;
      else
        raw = await req.ReadAsStringAsync();

      if (!string.IsNullOrWhiteSpace(raw))
        return JsonSerializer.Deserialize<T>(raw, DefaultJson);

      return default;
    }
    catch (Exception ex)
    {
      if (onError is not null)
        await onError(ex, raw ?? string.Empty);

      return default;
    }
  }
}
