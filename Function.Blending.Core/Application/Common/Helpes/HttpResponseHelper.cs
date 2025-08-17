using System.Net;
using System.Text.Json;
using Function.Blending.Core.Application.Common.Wrappers;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Common.Helpers;

public static class HttpResponseHelper
{
    public static async Task<HttpResponseData> WriteBaseResponseAsync<T>(HttpRequestData req, BaseResponse<T> response)
    {
        var res = req.CreateResponse((HttpStatusCode)response.StatusCode);

        // Serialización JSON normal
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        res.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await res.WriteStringAsync(json);

        return res;
    }
}