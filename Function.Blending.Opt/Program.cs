using Function.Blending.Opt.Application.Services;
using Function.Blending.Opt.Infrastructure.Data;
using Function.Blending.Opt.Mappings;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Trigger deployment - Opt function updated

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();

        // Filtro espec�fico para suprimir logs de AutoMapper.LicenseValidator
        logging.AddFilter((category, level) =>
        {
            if (category.Contains("AutoMapper.LicenseValidator"))
                return false; // Suprime todo log de esa categor�a

            return true; // Permite el resto
        });
    })
    .ConfigureServices(services =>
    {
        services.AddDbContext<BlendingDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));
        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>()); // Fix: Use a lambda to configure AutoMapper
        services.AddScoped<ModelExecutionService>();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();// Deploy trigger Tue Sep  2 08:07:59 PDT 2025
