using System.Net;
using System.Text.Json.Nodes;
using Function.Blending.Upload.Helpers.Excel;
using Function.Blending.Upload.Helpers.Json;
using Function.Blending.Upload.Infrastructure.Config.Output;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Services;
using Function.Blending.Upload.Services.Write.Quality;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Process;

public class WriteExcelQualityProcess
{
    private readonly ILogger<WriteExcelQualityProcess> _logger;
    private readonly XlsmProcessingService<ExcelMappingOutputQualityConfig> _xlsmProcessingService;
    private readonly IConfiguration _configuration;
    private readonly BlobStorageService _blobStorageService;

    public WriteExcelQualityProcess(ILogger<WriteExcelQualityProcess> logger, IConfiguration configuration,
        BlobStorageService blobStorageService)
    {
        _logger = logger;
        _xlsmProcessingService = new XlsmProcessingService<ExcelMappingOutputQualityConfig>(
            configuration["Template_Directory"] ??
            throw new ArgumentNullException("Template_Directory no está configurado."),
            configuration["Template_ExcelMappingOutputQuality"] ??
            throw new ArgumentNullException("Template_ExcelMappingOutputQuality no está configurado."));

        _configuration = configuration;
        _blobStorageService = blobStorageService;
    }
    
    public async Task<HttpResponseData> ExecuteAsync(HttpRequestData req)
{
    var body = await req.ReadAsStringAsync();
    if (string.IsNullOrWhiteSpace(body))
    {
        _logger.LogInformation("Body is empty");
        throw new ArgumentException("Request body cannot be null or empty.");
    }

    var deserializeJson = JsonHelper.Deserialize<WriteObjectDto>(body);
    if (deserializeJson == null)
    {
        const string msg = "No se pudo deserializar el body a WriteObjectDto.";
        _logger.LogError(msg);
        throw new ArgumentException(msg);
    }

    var config = _xlsmProcessingService.ConfiguracionActual;
    using var workbook = ExcelXMLHelper.GetWorkbook(
        _configuration["Template_Directory"] ??
        throw new ArgumentNullException("Template_Directory no está configurado."),
        _configuration["Template_QualityOutput"] ??
        throw new ArgumentNullException("Template_QualityOutput no está configurado."));

    if (workbook == null)
    {
        var msg = "No se pudo cargar el workbook desde la plantilla configurada.";
        _logger.LogError(msg);
        throw new ArgumentException(msg);
    }

    // Rellenar hojas
    var hojaResumen = workbook.Worksheet(config.Resumen.SheetName);
    var startRowResumen = config.Resumen.StartRow;

    ExcelWriteQualityService.Execute(hojaResumen, deserializeJson.grupos, config.Resumen.Fijos,
        startRowResumen, true);
    ExcelWriteQualityService.Execute(hojaResumen, deserializeJson.grupos,
        config.Resumen.ParametrosCalidad, startRowResumen, true);

    var hojaDetalle = workbook.Worksheet(config.Detalle.SheetName);
    var startRowDetalle = config.Detalle.StartRow;

    ExcelWriteQualityService.Execute(hojaDetalle, deserializeJson.detalles, config.Detalle.Fijos,
        startRowDetalle);
    ExcelWriteQualityService.Execute(hojaDetalle, deserializeJson.detalles,
        config.Detalle.ParametrosCalidad, startRowDetalle);

    // Guardar en memoria y responder
    using var memoryStream = new MemoryStream();
    workbook.SaveAs(memoryStream);
    memoryStream.Position = 0;

    var response = req.CreateResponse(HttpStatusCode.OK);
    response.Headers.Add("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    response.Headers.Add("Content-Disposition", $"attachment; filename=quality-report.xlsx");
    await response.WriteBytesAsync(memoryStream.ToArray());

    return response;
}

}