using System.Text.Json;
using ClosedXML.Excel;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Function.Blending.Upload.Services;

public class XlsmProcessingService<TConfig> where TConfig : class
{
    private readonly TConfig _config;
    public TConfig ConfiguracionActual => _config;

    public XlsmProcessingService(string directory , string configFileName)
    {
        var yamlPath = Path.Combine(Directory.GetCurrentDirectory(), directory, configFileName);
        var yamlText = File.ReadAllText(yamlPath);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _config = deserializer.Deserialize<TConfig>(yamlText);
    }
    
    public async Task<ParsedRowLogisticDto> ProcesarArchivoLogistcAsync(byte[] fileBytes)
    {
        var validateExcel = await ExcelValidateHelper.EnsureXlsxAsync(fileBytes);
        using var stream = new MemoryStream(validateExcel);
        var config = ConfigHelper.CastConfig<ExcelMappingLogisticsConfig>(_config);
        return await ExcelReaderHelper.LeerFilasLoigisticDesdeExcelAsync(stream.ToArray(), config);
    }
    
    public async Task<WriteLogisticObjectDto?> JsonDeserialize(string json)
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<WriteLogisticObjectDto>(json, options);
    }
    
    public XLWorkbook? GetXLWorkbookAction(string directory , string configFileName)
    {
        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), directory);
        var excelPath = Path.Combine(rootPath, configFileName);
        return new XLWorkbook(excelPath);
    } 
    
    public async Task<List<ParsedRowDto>> ProcesarArchivoAsync(byte[] fileBytes)
    {
        var validateExcel = await ExcelValidateHelper.EnsureXlsxAsync(fileBytes);
        using var stream = new MemoryStream(validateExcel);
        var config = ConfigHelper.CastConfig<ExcelMappingConfig>(_config);
        return await ExcelReaderHelper.LeerFilasDesdeExcelAsync(stream.ToArray(), config);
    }
    
}