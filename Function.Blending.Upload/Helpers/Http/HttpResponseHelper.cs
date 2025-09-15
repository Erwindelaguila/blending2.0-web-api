using System.Net;
using System.Text.Json;
using Function.Blending.Upload.Response;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Upload.Helpers.Http;

public static class HttpResponseHelper
{
    public static async Task<HttpResponseData> WriteBaseResponseAsync<T>(HttpRequestData req, BaseResponse<T> response)
    {
        var res = req.CreateResponse((HttpStatusCode)response.StatusCode);

        // Serializar manualmente con opciones en español o camelCase
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        res.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await res.WriteStringAsync(json);

        return res;
    }
}