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

  public static async Task<T?> TryReadFromJsonOrRawAsync<T>(
    this HttpRequestData req,
    FunctionContext ctx,
    CancellationToken cancellationToken = default)
  {
    try
    {
      return await ReadFromJsonOrRawAsync<T>(req, ctx, cancellationToken);
    }
    catch
    {
      return default;
    }
  }
}
