using System.Web;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Excel;
using Function.Blending.Upload.Helpers.Mapper;
using Function.Blending.Upload.Helpers.Multipart;
using Function.Blending.Upload.Helpers.Xml;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Services;
using Function.Blending.Upload.Services.Write.Sap;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Process;

public class SapStockProcess
{
    private readonly BlobStorageService _blobStorageService;
    private readonly ILogger<SapStockProcess> _logger;
    private readonly SapStockService _sapStockService;
    private readonly SapXmlHelper _sapXmlParser;
    private readonly XlsmProcessingService<ExcelMappingOutputSapConfig> _xlsmProcessingService;
    private readonly IConfiguration _configuration;
    private readonly ExcelQualityProcessorService _excelQualityProcessorService;


    public SapStockProcess(BlobStorageService blobStorageService, ILogger<SapStockProcess> logger,
        IHttpClientFactory httpClientFactory, SapXmlHelper sapXmlParser, IConfiguration configuration,
        ExcelQualityProcessorService excelQualityProcessorService)
    {
        _sapStockService = new SapStockService(httpClientFactory);
        _blobStorageService = blobStorageService;
        _sapXmlParser = sapXmlParser;
        _logger = logger;
        _configuration = configuration;
        _xlsmProcessingService =
            new XlsmProcessingService<ExcelMappingOutputSapConfig>(
                configuration["Template_Directory"] ??
                throw new ArgumentNullException("Template_Directory no está configurado."),
                configuration["Template_ExcelMappingInputSap"] ??
                throw new ArgumentException("Template_ExcelMappingInputSap no esta onfigurado"));
        _excelQualityProcessorService = excelQualityProcessorService;
    }

    public async Task<BlobResultDto> ExecuteAsync(HttpRequestData req)
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
        using var workbook =
            ExcelXMLHelper.GetWorkbook(
                _configuration["Template_Directory"] ??
                throw new ArgumentNullException("Template_Directory no está configurado."),
                _configuration["Template_SapOutput"] ??
                throw new ArgumentNullException("Template_SapOutput no está configurado."));
        
        ExcelWriteSapService.Execute(config, parsedJson, workbook);
        
        var fileBytes = await MultipartFormDataHelper.ToByteArrayAsync(workbook);

        var resultList = await _excelQualityProcessorService.ExecuteAsync(fileBytes, req);

        var blobResult = await _blobStorageService.UploadExcelAndGetLinkAsync(workbook, "sap-stock");

        blobResult.DataExcel = resultList;

        return blobResult;
    }
}