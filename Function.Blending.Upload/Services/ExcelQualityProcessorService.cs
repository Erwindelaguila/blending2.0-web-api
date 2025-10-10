using Function.Blending.Upload.Helpers.Parsed;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Infrastructure.Config.Input;
using Function.Blending.Upload.Models;
using Function.Blending.Upload.Services;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Upload.Helpers.Mapper;

public class ExcelQualityProcessorService
{
    private readonly XlsmProcessingService<ExcelMappingInputQualityConfig> _xlsmProcessingServiceExcel;
    private readonly CalidadService _calidadService;

    public ExcelQualityProcessorService(
        CalidadService calidadService, IConfiguration configuration)
    {
        _xlsmProcessingServiceExcel =
            new XlsmProcessingService<ExcelMappingInputQualityConfig>(configuration["Template_Directory"] ?? throw new ArgumentNullException("Template_Directory no esta configurado."),
                configuration["Template_ExcelMappingInputQuality"] ?? throw new ArgumentNullException("Template_ExcelMappingInputQuality no esta configurado."));
        _calidadService = calidadService;
    }

    public async Task<List<ExcelExtractQualityDto>> ExecuteAsync(byte[] fileBytes, HttpRequestData req)
    {
        
        var configActualExcel = _xlsmProcessingServiceExcel.ConfiguracionActual;
        var filas = await _xlsmProcessingServiceExcel.ProcesarArchivoQualityAsync(fileBytes);
        var listaCalidades = await _calidadService.GetCalidadAsync(req);
        
        return filas
            .Where(ParsedRowValidator.EsValido)
            .Select(fila => MapperQualityHepler.Execute(fila, configActualExcel, listaCalidades))
            .ToList();
    }
}