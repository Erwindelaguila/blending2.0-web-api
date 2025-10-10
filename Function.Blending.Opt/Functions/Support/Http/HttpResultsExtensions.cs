using System.Net;
using Microsoft.Azure.Functions.Worker.Http;
using Function.Blending.Opt.Functions.Support.Extensions; // <- para WriteJsonAsync
using Function.Blending.Opt.Shared.Json; // opcional si quisieras usar JsonDefaults directamente

namespace Function.Blending.Opt.Functions.Support.Http;

public static class HttpResultsExtensions
{
  public static async Task<HttpResponseData> OkAsync<T>(this HttpRequestData req, T body)
  {
    var res = req.CreateResponse(HttpStatusCode.OK);
    // IMPORTANTE: usar nuestro WriteJsonAsync (JsonDefaults.Web) en vez de WriteAsJsonAsync
    await res.WriteJsonAsync(ApiResponse<T>.Of(body));
    return res;
  }

  // OK con meta
  public static async Task<HttpResponseData> OkAsync<T>(this HttpRequestData req, T body, object? meta)
  {
    var res = req.CreateResponse(HttpStatusCode.OK);
    await res.WriteJsonAsync(new ApiResponse<T> { Success = true, Data = body, Meta = meta });
    return res;
  }

  public static HttpResponseData WithStatus(this HttpRequestData req, HttpStatusCode status)
      => req.CreateResponse(status);

  public static async Task<HttpResponseData> JsonAsync<T>(this HttpRequestData req, HttpStatusCode status, T body)
  {
    var res = req.CreateResponse(status);
    await res.WriteJsonAsync(body);
    return res;
  }
}
