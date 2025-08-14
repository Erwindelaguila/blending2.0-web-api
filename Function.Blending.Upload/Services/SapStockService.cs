using System.Net.Http.Headers;
namespace Function.Blending.Upload.Services;

public class SapStockService
{
    private readonly HttpClient _httpClient;

    public SapStockService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<string> GetStockXmlAsync()
    {
        var sapUrl = "https://centria.apimanagement.br1.hana.ondemand.com/QAS/Tasa/odata/ZQMSO_DISTRI_STOCK_SRV/Ent_StockDispSet";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("SAP_API_KEY"));

        var response = await _httpClient.GetAsync(sapUrl);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Error al contactar SAP");
        }

        return await response.Content.ReadAsStringAsync();
    }
}