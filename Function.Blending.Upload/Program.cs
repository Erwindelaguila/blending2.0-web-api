using Function.Blending.Upload.Functions.Process;
using Function.Blending.Upload.Functions.Support.Extensions;
using Function.Blending.Upload.Helpers.Mapper;
using Function.Blending.Upload.Helpers.Xml;
using Function.Blending.Upload.Infrastructure.Security.KeyDecoders;
using Function.Blending.Upload.Services;
using Function.Blending.Upload.Shared.Security;
using FunctionBlending.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ConfigurationKeys = Function.Blending.Upload.Shared.Constants.ConfigurationKeys;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
// Helper para flags desde config
bool on(string key, bool def = true) => bool.TryParse(builder.Configuration[key], out var b) ? b : def;
builder.Services.AddFunctionsSupport(builder.Configuration);


builder.Services.AddSingleton<IKeyDecoder, KeyDecoder>();

// Registrar Application Insights (opcional)!!!
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Registrar HttpClientFactory para inyección de IHttpClientFactory
builder.Services.AddSingleton<BlobStorageService>();
builder.Services.AddSingleton<SapStockProcess>();
builder.Services.AddSingleton<SapXmlHelper>();
builder.Services.AddHttpClient<CadmioService>();
builder.Services.AddHttpClient<CalidadService>();
builder.Services.AddScoped<ExcelQualityProcessorService>();
builder.Services.AddScoped<SapStockProcess>();
builder.Services.AddScoped<GetCadmioProcess>();
builder.Services.AddScoped<UploadExcelLogisticProcess>();
builder.Services.AddScoped<UploadExcelQualityProcess>();
builder.Services.AddScoped<WriteExcelLogisticsProcess>();
builder.Services.AddScoped<WriteExcelQualityProcess>();

builder.UseFunctionsPipeline(
    enableExceptionHandling: on(ConfigurationKeys.Pipeline.EnableExceptionHandling),
    enableRequestLogging: on(ConfigurationKeys.Pipeline.EnableRequestLogging),
    enableRequestSizeLimit: on(ConfigurationKeys.Pipeline.EnableRequestSizeLimit),
    enableAuthentication: on(ConfigurationKeys.Pipeline.EnableAuthentication)
);


// Registrar otros servicios si es necesario
// builder.Services.AddSingleton<XlsmProcessingService>();
builder.Build().Run();