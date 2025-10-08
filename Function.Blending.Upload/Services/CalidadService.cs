using System.Net.Http.Headers;
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
        _baseUrl = configuration["Service_CoreService"] 
            ?? throw new ArgumentNullException("Service_CoreService not configured");
    }

    public async Task<List<CalidadDto>> GetCalidadAsync(HttpRequestData req)
    {
        var requestMessage = new HttpRequestMessage(
            HttpMethod.Get, 
            $"{_baseUrl}/api/core/produccion/calidad?isGlobal=1"
        );

        // ✅ Reenvía el token de autorización si existe
        if (req.Headers.TryGetValues("Authorization", out var authHeaders))
        {
            var token = authHeaders.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(token))
            {
                // Si viene como "Bearer abc123..."
                requestMessage.Headers.Authorization = 
                    AuthenticationHeaderValue.Parse(token);
            }
        }

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Error en la llamada a coreServices para obtener las descripciones de calidades. " +
                $"Status: {(int)response.StatusCode}, Content: {errorContent}"
            );
        }

        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<ApiResponseDto<CalidadDto>>(json, options);

        return result?.Data ?? new List<CalidadDto>();
    }
}
