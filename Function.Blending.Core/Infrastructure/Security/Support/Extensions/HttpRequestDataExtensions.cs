using System.Text.Json;
using Function.Blending.Upload.Shared.Json;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Infrastructure.Security.Support.Extensions;

public static class HttpRequestDataExtensions
{
  public static async Task<T?> TryReadJsonAsync<T>(this HttpRequestData req)
  {
    var body = await req.ReadAsStringAsync();
    if (string.IsNullOrWhiteSpace(body)) return default;
    try { return JsonDefaults.Deserialize<T>(body); }
    catch { return default; }
  }

  public static Uri BuildLocation(this HttpRequestData req, string relativeRoute)
    => new(req.Url, relativeRoute);

  public static async Task<HttpResponseData> AcceptedWithLocationAsync(this HttpRequestData req, Uri location, object body)
  {
    var res = req.CreateResponse(System.Net.HttpStatusCode.Accepted);
    res.Headers.Add("Location", location.ToString());
    await res.WriteJsonAsync(body);
    return res;
  }

  public static async Task WriteJsonAsync<T>(this HttpResponseData res, T value)
  {
    res.Headers.Add("Content-Type", "application/json; charset=utf-8");
    var json = JsonSerializer.Serialize(value, JsonDefaults.Web);
    await res.WriteStringAsync(json);
  }
}
