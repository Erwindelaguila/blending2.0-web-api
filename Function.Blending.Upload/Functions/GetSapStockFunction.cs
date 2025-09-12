using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Multipart;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Response;
using Function.Blending.Upload.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;


public class GetSapStockFunction
{
    private readonly SapStockService _sapStockService;
    private readonly SapXmlHelper _sapXmlParser;
    private readonly XlsmProcessingService<ExcelSapMappingOutputConfig> _xlsmProcessingService;
    private readonly XlsmProcessingService<ExcelMappingConfig> _xlsmProcessingServiceExcel;
    private readonly ILogger _logger;
    private readonly BlobStorageService _blobStorageService;
    private readonly CalidadService _calidadService;

    public GetSapStockFunction(
        IHttpClientFactory httpClientFactory,
        ILogger<GetSapStockFunction> logger,
        BlobStorageService blobStorageService,
        CalidadService calidadService
    )
    {
        _sapStockService = new SapStockService(httpClientFactory);
        _sapXmlParser = new SapXmlHelper();
        _logger = logger;
        _xlsmProcessingService =
            new XlsmProcessingService<ExcelSapMappingOutputConfig>("Templates", "ExcelSapMappingInput.yml");
        _blobStorageService = blobStorageService;
        _xlsmProcessingServiceExcel =
            new XlsmProcessingService<ExcelMappingConfig>("Templates", "ExcelMappingInput.yaml");
        _calidadService = calidadService;
    }

    [Function("GetSapStockFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "upload/get-sap-stock")]
        HttpRequestData req)
    {
        try
        {
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var fileName = query["fileName"];

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                _logger.LogInformation($"Intentando eliminar archivo existente: {fileName}");
                var deleted = await _blobStorageService.DeleteFileIfExistsAsync(fileName);
                _logger.LogInformation(deleted
                    ? $"Archivo '{fileName}' eliminado correctamente del blob storage."
                    : $"Archivo '{fileName}' no encontrado o ya eliminado.");
            }

            var xml = await _sapStockService.GetStockXmlAsync();

            var parsedJson = _sapXmlParser.ParseStockXml(xml);

            var config = _xlsmProcessingService.ConfiguracionActual;
            var configActualExcel = _xlsmProcessingServiceExcel.ConfiguracionActual;

            using var workbook = _xlsmProcessingService.GetXLWorkbookAction("Templates", "template_sap_output.xlsx");

            ExcelWriterService.WriteExcelSap(config, parsedJson, workbook);
            var fileBytes = await MultipartFormDataHelper.ToByteArrayAsync(workbook);
            
            //Refactor 

            var filas = await _xlsmProcessingServiceExcel.ProcesarArchivoAsync(fileBytes);

            var listaCalidades = await _calidadService.GetCalidadAsync(req);

            var listaFinal = filas
                .Where(ParsedRowValidator.EsValido)
                .Select(fila => ParsedRowMapperHelper.Mapear(fila, configActualExcel, listaCalidades))
                .ToList();
            
            // fin refactor

            var blobResult = await _blobStorageService.UploadExcelAndGetLinkAsync(workbook);

            blobResult.ExcelDataSap = listaFinal;

            return await HttpResponseHelper.WriteBaseResponseAsync(
                req,
                BaseResponse<BlobResultDto>.Success(
                    blobResult,
                    "Datos obtenidos correctamente")
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar solicitud SAP");

            var error = new
            {
                Message = "Ocurrió un error inesperado.",
                Exception = ex.Message,
                InnerException = ex.InnerException?.Message,
                TraceContext = ex.StackTrace
            };

            return await HttpResponseHelper.WriteBaseResponseAsync(
                req,
                BaseResponse<object>.Fail(error, ex.Message, 500)
            );
        }
    }
}