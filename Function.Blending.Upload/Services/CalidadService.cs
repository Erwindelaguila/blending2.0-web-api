using System.Net.Http;
using System.Text.Json;
using Function.Blending.Upload.Models;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Upload.Services;

public class CalidadService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public CalidadService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["Service_CoreService"] ?? throw new ArgumentNullException("Service_CoreService not configured");
    }

    public async Task<List<CalidadDto>> GetCalidadAsync(HttpRequestData req)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/api/core/produccion/calidad?isGlobal=1");

        // 🔽 Si quieres reenviar headers entrantes
        /*
        foreach (var header in req.Headers)
        {
            if (!requestMessage.Headers.Contains(header.Key))
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }
        */

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            //var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Error en la llamada a coreServices para obtener las descripciones de calidades"
            );
        }

        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<ApiResponseDto<CalidadDto>>(json, options);

        return result?.Data ?? new List<CalidadDto>();
    }
}