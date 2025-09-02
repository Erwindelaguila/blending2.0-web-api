using Function.Blending.Upload.Services;
using FunctionBlending.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Registrar Application Insights (opcional)
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Registrar HttpClientFactory para inyección de IHttpClientFactory
builder.Services.AddSingleton<BlobStorageService>();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<CadmioService>();

// Registrar otros servicios si es necesario
// builder.Services.AddSingleton<XlsmProcessingService>();

builder.Build().Run();// Deploy trigger Tue Sep  2 08:08:20 PDT 2025
