using System.Net.Http;
using System.Text;
using System.Text.Json;
using Function.Blending.Upload.Models;
using System.Net.Http.Json;

namespace FunctionBlending.Core.Services
{
    public class CadmioService
    {
        private readonly HttpClient _httpClient;

        public CadmioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CadmioResult>> ObtenerCadmioAsync(ObtenerCadmioRequestDto request)
        {
            // Mapear rumas -> SAP DTO
            var sapRequest = new ObtenerCadmioSapRequestDto
            {
                ZSDF_BLENDING_GET_CADMIO = new ZsdfBlendingGetCadmio
                {
                    IT_RUMAS = new ItRumas
                    {
                        item = request.Rumas.Select(r => new ItemRuma { CHARG = r }).ToList()
                    }
                }
            };

            // Configuración de serialización para respetar exactamente los nombres de las propiedades
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null, // respeta el nombre de la clase tal cual
                WriteIndented = true
            };

            // Serializar el objeto a JSON
            var json = JsonSerializer.Serialize(sapRequest, options);

            // Log para comparar con Postman
            Console.WriteLine("JSON enviado a SAP:\n" + json);

            // Crear request HTTP
            var httpRequest = new HttpRequestMessage(HttpMethod.Post,
                "https://centria.apimanagement.br1.hana.ondemand.com:443/QAS/Tasa/Blending/obtener_cadmio");

            httpRequest.Headers.Add("x-api-key", "ST3xF0AL8LfhYZaqyCIaiCcBGGaTBaOF");
            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            var rawContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"SAP Error {response.StatusCode}: {rawContent}");
                return new List<CadmioResult>();
            }
            
            if (rawContent.Contains("\"ET_DATA\":\"\""))
            {
                rawContent = rawContent.Replace("\"ET_DATA\":\"\"", "\"ET_DATA\":{}");
            }
            

            // TODO: Ajustar parseo según la respuesta real de SAP
            var sapResponse = JsonSerializer.Deserialize<ObtenerCadmioSapResponseDto>(rawContent, options);

            var results = sapResponse?.ZSDF_BLENDING_GET_CADMIOResponse?.ET_DATA?.item
                .Select(i => new CadmioResult
                {
                    RumaNro = i.CHARG,
                    Valor = i.CADMIO
                }).ToList() ?? new();

            return results;
        }
    }
}
