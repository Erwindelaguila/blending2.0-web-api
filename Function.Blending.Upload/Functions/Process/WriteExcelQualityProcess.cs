using System.Text.Json.Nodes;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Excel;
using Function.Blending.Upload.Helpers.Json;
using Function.Blending.Upload.Infrastructure.Config;
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

    public WriteExcelQualityProcess(ILogger<WriteExcelQualityProcess> logger, IConfiguration configuration,BlobStorageService blobStorageService)
    {
        _logger = logger;
        _xlsmProcessingService = new XlsmProcessingService<ExcelMappingOutputQualityConfig>(configuration["Template:Directory"], configuration["Template:ExcelMappingOutputQuality"]);
        _configuration = configuration;
        _blobStorageService = blobStorageService;
    }

    public async Task<BlobResultDto> ExecuteAsync(HttpRequestData req)
    {
        var body = await req.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
        {
            _logger.LogInformation("Body is empty");
            throw new ArgumentException("Request body cannot be null or empty.");
        }
        
        var json = JsonHelper.Deserialize<WriteObjectDto>(body);
        var config = _xlsmProcessingService.ConfiguracionActual;
        using var workbook = ExcelXMLHelper.GetWorkbook(_configuration["Template:Directory"], _configuration["Template:QualityOutput"]);
        
        var hojaResumen = workbook.Worksheet(config.Resumen.SheetName);
        var startRowResumen = config.Resumen.StartRow;
        
        ExcelWriteQualityService.Execute<JsonObject>(hojaResumen, json.resultado.grupos, config.Resumen.Fijos, startRowResumen, true);
        ExcelWriteQualityService.Execute<JsonObject>(hojaResumen, json.resultado.grupos, config.Resumen.ParametrosCalidad, startRowResumen, true);
        
        var hojaDetalle = workbook.Worksheet(config.Detalle.SheetName);
        var startRowDetalle = config.Detalle.StartRow;

        ExcelWriteQualityService.Execute<JsonObject>(hojaDetalle, json.resultado.rumas, config.Detalle.Fijos, startRowDetalle); 
        ExcelWriteQualityService.Execute<JsonObject>(hojaDetalle, json.resultado.rumas, config.Detalle.ParametrosCalidad, startRowDetalle);
        var blobResult = await _blobStorageService.UploadExcelAndGetLinkAsync(workbook, "quality-report");
        blobResult.DataExcel = null;
        _logger.LogInformation($"Reporte de Excel de calidad guardado con exito con el nombre:{blobResult.FileName}");
        return blobResult;
    }
}