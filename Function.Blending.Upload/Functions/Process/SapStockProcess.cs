using System.Web;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Multipart;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Services;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Process;

public class SapStockProcess
{
    private readonly BlobStorageService _blobStorageService;
    private readonly ILogger<GetSapStockFunction> _logger;
    private readonly SapStockService _sapStockService;
    private readonly SapXmlHelper _sapXmlParser;
    private readonly XlsmProcessingService<ExcelSapMappingOutputConfig> _xlsmProcessingService;
    private readonly XlsmProcessingService<ExcelMappingConfig> _xlsmProcessingServiceExcel;
    private readonly CalidadService _calidadService;
    private readonly IConfiguration _configuration; 
    

    public SapStockProcess(BlobStorageService blobStorageService, ILogger<GetSapStockFunction> logger,
        IHttpClientFactory httpClientFactory, SapXmlHelper sapXmlParser, IConfiguration configuration,  CalidadService calidadService)
    {
        _sapStockService = new SapStockService(httpClientFactory);
        _blobStorageService = blobStorageService;
        _sapXmlParser = sapXmlParser;
        _logger = logger;
        _configuration = configuration;
        _xlsmProcessingService =
            new XlsmProcessingService<ExcelSapMappingOutputConfig>(configuration["Template:Directory"], configuration["Template:ExcelSapYML"]);
        _xlsmProcessingServiceExcel =
            new XlsmProcessingService<ExcelMappingConfig>(configuration["Template:Directory"], configuration["Template:ExcelMappingInputQuality"]);
    }

    public async Task<List<string>> ExecuteAsync(HttpRequestData req)
    {
        var query = HttpUtility.ParseQueryString(req.Url.Query);

        var fileName = query["fileName"];
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            _logger.LogInformation($"Intentando eliminar archivo existente: {fileName}");
            var deleted = await _blobStorageService.DeleteFileIfExistsAsync(fileName);
            _logger.LogInformation(deleted
                ? $"Archivo '{fileName}' eliminado correctamente del blob storage."
                : $"Archivo '{fileName}' no encontrado o ya eliminado.");
        }

        var xmlProcess = await _sapStockService.GetStockXmlAsync();
        var parsedJson = _sapXmlParser.ParseStockXml(xmlProcess);
        var config = _xlsmProcessingService.ConfiguracionActual;
        var configActualExcel = _xlsmProcessingServiceExcel.ConfiguracionActual;
        using var workbook = ExcelXMLHelper.GetWorkbook(_configuration["Template:Directory"], _configuration["Template:SapOutput"]);
        ExcelWriterService.WriteExcelSap(config, parsedJson, workbook);
        var fileBytes = await MultipartFormDataHelper.ToByteArrayAsync(workbook);


        return null;
    }
}