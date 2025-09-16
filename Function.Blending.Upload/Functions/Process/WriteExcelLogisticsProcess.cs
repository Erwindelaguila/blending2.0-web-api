using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Excel;
using Function.Blending.Upload.Helpers.Json;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Infrastructure.Config.Output;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Services;
using Function.Blending.Upload.Services.Write.Logistic.containers;
using Function.Blending.Upload.Services.Write.Logistic.Sap;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Process;

public class WriteExcelLogisticsProcess
{
    private readonly ILogger<WriteExcelLogisticsProcess> _logger;
    private readonly XlsmProcessingService<ExcelMappingOutputLogisticsConfig> _xlsmProcessingService;
    private readonly BlobStorageService _blobStorageService;
    private readonly IConfiguration _configuration;

    public WriteExcelLogisticsProcess(ILogger<WriteExcelLogisticsProcess> logger, IConfiguration configuration,
        BlobStorageService blobStorageService)
    {
        _logger = logger;
        _blobStorageService = blobStorageService;
        _xlsmProcessingService =
            new XlsmProcessingService<ExcelMappingOutputLogisticsConfig>(
                configuration["Template_Directory"] ??
                throw new ArgumentNullException("Template_Directory no está configurado."),
                configuration["Template_ExcelMappingOutputLogistic"] ?? throw new ArgumentNullException("Template_ExcelMappingOutputLogistic no está configurado."));
        _configuration = configuration;
    }

    public async Task<BlobResultDto> ExecuteAsync(HttpRequestData req)
    {
        _logger.LogInformation($"Iniciando proceso de reporte de logistica .....");
        var body = await req.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
        {
            _logger.LogInformation("Body is empty");
            throw new ArgumentException("Request body cannot be null or empty.");
        }

        var json = JsonHelper.Deserialize<WriteLogisticObjectDto>(body);
        var config = _xlsmProcessingService.ConfiguracionActual;

        var dataContHomogenizacion = json.Data;
        using var workbook = ExcelXMLHelper.GetWorkbook(
            _configuration["Template_Directory"] ??
            throw new ArgumentNullException("Template_Directory no está configurado."),
            _configuration["Template_LogisticOutput"] ?? throw new ArgumentNullException("Template_LogisticOutput no está configurado."));

        ExcelWriteContainersLogisticService.Execute(config, dataContHomogenizacion, workbook);
        ExcelWriteSapLogisticService.Execute(config, dataContHomogenizacion, workbook);
        var blobResult = await _blobStorageService.UploadExcelAndGetLinkAsync(workbook, "logistic-report");
        blobResult.DataExcel = null;
        _logger.LogInformation(
            $"Reporte de Excel de logistica guardado con exito con el nombre: {blobResult.FileName}");
        return blobResult;
    }
}