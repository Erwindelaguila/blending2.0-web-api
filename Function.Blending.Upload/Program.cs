using Function.Blending.Upload.Services;
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

// Registrar otros servicios si es necesario
// builder.Services.AddSingleton<XlsmProcessingService>();

builder.Build().Run();