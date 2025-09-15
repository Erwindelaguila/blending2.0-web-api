using Function.Blending.Upload.Functions.Process;
using Function.Blending.Upload.Helpers;
using Function.Blending.Upload.Helpers.Mapper;
using Function.Blending.Upload.Helpers.Xml;
using Function.Blending.Upload.Services;
using FunctionBlending.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

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


// Registrar otros servicios si es necesario
// builder.Services.AddSingleton<XlsmProcessingService>();
builder.Build().Run();