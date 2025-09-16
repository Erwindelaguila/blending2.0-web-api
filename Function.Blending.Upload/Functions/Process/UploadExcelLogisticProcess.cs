using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Mapper;
using Function.Blending.Upload.Helpers.Multipart;
using Function.Blending.Upload.Helpers.Parsed;
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

    public UploadExcelLogisticProcess(IConfiguration configuration, ILogger<GetSapStockFunction> logger)
    {
        _logger = logger;
        _xlsmProcessingService =
            new XlsmProcessingService<ExcelMappingInputLogisticsConfig>(
                configuration["Template_Directory"] ??
                throw new ArgumentNullException("Template_Directory no está configurado."),
                configuration["Template_ExcelMappingInputLogistic"] ??
                throw new ArgumentNullException("Template_ExcelMappingInputLogistic no está configurado."));
    }

    public async Task<ExcelExtractLogisticDto> ExecuteAsync(HttpRequestData req)
    {
        if (!MultipartRequestValidator.IsMultipartFormData(req))
        {
            _logger.LogError("El tipo de contenido debe ser multipart/form-data.");
            throw new InvalidOperationException($"El tipo de contenido debe ser multipart/form-data.");
        }

        var fileBytes = await MultipartFormDataHelper.ExtractFileAsync(req);

        var dataLogistic = await _xlsmProcessingService.ProcesarArchivoLogistcAsync(fileBytes);

        var config = _xlsmProcessingService.ConfiguracionActual;

        return MapperLogisticHelper.Execute(dataLogistic, config);
    }
}