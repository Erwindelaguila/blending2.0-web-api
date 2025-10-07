using System.IO;
using System.Web;
using ClosedXML.Excel;
using Function.Blending.Upload.Functions.Triggers.Sap;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Excel;
using Function.Blending.Upload.Helpers.Mapper;
using Function.Blending.Upload.Helpers.Multipart;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Infrastructure.Config.Input;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Services;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Process;

public class UploadExcelLogisticProcess
{
    private readonly XlsmProcessingService<ExcelMappingInputLogisticsConfig> _xlsmProcessingService;
    private readonly ILogger<GetSapStockFunction> _logger;
    private readonly BlobStorageService _blobStorageService;

    public UploadExcelLogisticProcess(
        IConfiguration configuration,
        ILogger<GetSapStockFunction> logger,
        BlobStorageService blobStorageService)
    {
        _logger = logger;
        _blobStorageService = blobStorageService;

        _xlsmProcessingService =
            new XlsmProcessingService<ExcelMappingInputLogisticsConfig>(
                configuration["Template_Directory"] ??
                throw new ArgumentNullException("Template_Directory no está configurado."),
                configuration["Template_ExcelMappingInputLogistic"] ??
                throw new ArgumentNullException("Template_ExcelMappingInputLogistic no está configurado."));
    }

    public async Task<BlobResultDto<ExcelExtractLogisticDto>> ExecuteAsync(HttpRequestData req)
    {
        if (!MultipartRequestValidator.IsMultipartFormData(req))
        {
            _logger.LogError("El tipo de contenido debe ser multipart/form-data.");
            throw new InvalidOperationException($"El tipo de contenido debe ser multipart/form-data.");
        }
        
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
        

        // 1️⃣ Obtener bytes del Excel cargado
        var fileBytes = await MultipartFormDataHelper.ExtractFileAsync(req);
        var validateExcel = await ExcelValidateHelper.EnsureXlsxAsync(fileBytes);

        // 2️⃣ Crear un workbook (burbuk) desde esos bytes
        using var stream = new MemoryStream(validateExcel);
        using var workbook = new XLWorkbook(stream);

        // (opcional) procesas el Excel como ya lo haces:
        var dataLogistic = await _xlsmProcessingService.ProcesarArchivoLogistcAsync(validateExcel);
        var config = _xlsmProcessingService.ConfiguracionActual;
        var mappedData = MapperLogisticHelper.Execute(dataLogistic, config);

        // 3️⃣ Subir el workbook al blob storage
        var blobResult =
            await _blobStorageService.UploadExcelAndGetLinkAsync<ExcelExtractLogisticDto>(workbook, "asignacion-logistic-upload");

        // Puedes agregarle información adicional
        blobResult.DataExcel = mappedData;

        return blobResult;
    }
}