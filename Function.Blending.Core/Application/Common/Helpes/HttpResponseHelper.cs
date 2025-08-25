using System.Net;
using System.Text.Json;
using Function.Blending.Core.Application.Common.Wrappers;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Common.Helpers;

public static class HttpResponseHelper
{
    /// <summary>
    /// Opciones de serialización JSON consistentes para toda la aplicación
    /// </summary>
    /// <returns>JsonSerializerOptions configurado con camelCase</returns>
    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Opciones de deserialización JSON consistentes para toda la aplicación
    /// </summary>
    /// <returns>JsonSerializerOptions configurado con camelCase y case-insensitive</returns>
    public static JsonSerializerOptions GetJsonDeserializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    public static async Task<HttpResponseData> WriteBaseResponseAsync<T>(HttpRequestData req, BaseResponse<T> response)
    {
        try
        {
            var res = req.CreateResponse((HttpStatusCode)response.StatusCode);

            var json = JsonSerializer.Serialize(response, GetJsonSerializerOptions());

            res.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await res.WriteStringAsync(json);

            return res;
        }
        catch (ObjectDisposedException)
        {
            // Si el contexto está disposed, crear una respuesta fallback
            var fallbackRes = req.CreateResponse(HttpStatusCode.InternalServerError);
            var fallbackJson = JsonSerializer.Serialize(new
            {
                succeeded = false,
                message = "Request timeout o context disposed",
                statusCode = 500
            }, GetJsonSerializerOptions());
            
            try
            {
                fallbackRes.Headers.Add("Content-Type", "application/json; charset=utf-8");
                await fallbackRes.WriteStringAsync(fallbackJson);
            }
            catch
            {
                // Si aún falla, retornar la respuesta sin contenido
            }
            
            return fallbackRes;
        }
    }
}