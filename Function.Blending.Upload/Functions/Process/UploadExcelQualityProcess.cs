using Function.Blending.Upload.Helpers.Mapper;
using Function.Blending.Upload.Helpers.Multipart;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Services;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Upload.Functions.Process;

public class UploadExcelQualityProcess
{
    private readonly ILogger<UploadExcelQualityProcess> _logger;
    private readonly ExcelQualityProcessorService _excelQualityProcessorService;

    public UploadExcelQualityProcess(ILogger<UploadExcelQualityProcess> logger,
        ExcelQualityProcessorService excelQualityProcessorService )
    {
        _excelQualityProcessorService = excelQualityProcessorService;
        _logger = logger;   
    }

    public async Task<List<ExcelExtractQualityDto>> ExecuteAsync(HttpRequestData req)
    {
        if (!MultipartRequestValidator.IsMultipartFormData(req))
        {
            _logger.LogError("El tipo de contenido debe ser multipart/form-data.");
            throw new InvalidOperationException($"El tipo de contenido debe ser multipart/form-data.");
        }
        var fileBytes = await MultipartFormDataHelper.ExtractFileAsync(req);
        return await _excelQualityProcessorService.ExecuteAsync(fileBytes, req);
    }
}