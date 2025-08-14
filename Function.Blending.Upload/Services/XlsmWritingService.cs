using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Text.Json.Serialization;
using ClosedXML.Excel;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Models;

namespace Function.Blending.Upload.Services;

public class XlsmWritingService
{
    private readonly ExcelMappingOutputConfig _config;
    
    public ExcelMappingOutputConfig ConfiguracionActual => _config;

    public string DirectorioBase = "Templates";
    public XlsmWritingService()
    {

        var yamlPath = Path.Combine(Directory.GetCurrentDirectory(), DirectorioBase, "ExcelMappingOutput.yaml");
        var yamlText = File.ReadAllText(yamlPath);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _config = deserializer.Deserialize<ExcelMappingOutputConfig>(yamlText);

       
    }
    
    public async Task<WriteObjectDto?> JsonDeserialize()
    {
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), DirectorioBase, "data_output.json");

        if (!File.Exists(jsonPath))
            throw new FileNotFoundException($"Archivo no encontrado: {jsonPath}");

        string jsonContent = await File.ReadAllTextAsync(jsonPath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<WriteObjectDto>(jsonContent, options);
    }

    public XLWorkbook? GetXLWorkbook()
    {
        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
        var excelPath = Path.Combine(rootPath, "template_output.xlsx");
        return new XLWorkbook(excelPath);
    } 
    
    
}