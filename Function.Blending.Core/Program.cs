using FluentValidation;
using Function.Blending.Core.Application.Common.Behaviors;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Infrastructure.Mappings;
using Function.Blending.Core.Infrastructure.Persistence;
using Function.Blending.Core.Infrastructure.Persistence.Mappings;
using Function.Blending.Core.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();


        logging.AddFilter((category, level) =>
        {
            if (category != null && category.Contains("AutoMapper.LicenseValidator"))
                return false; 

            return true; 
        });
    })
    .ConfigureServices(services =>
    {
        services.AddDbContext<BlendingDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));
        
        // Memory Cache para optimización de performance
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 100; 
        });
        
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<ProductoProfile>();
            cfg.AddProfile<PlantaProfile>();
            cfg.AddProfile<CalidadProfile>();
            cfg.AddProfile<ParametroProfile>();
            cfg.AddProfile<AgregadoProfile>();
            cfg.AddProfile<LineaProduccionProfile>();
            cfg.AddProfile<TipoProduccionProfile>();
            // Removed GraphProfile reference to prevent build error
        });
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IPlantaRepository, PlantaRepository>();
        services.AddScoped<ICalidadRepository, CalidadRepository>();
        services.AddScoped<IParametroRepository, ParametroRepository>();
        services.AddScoped<IAgregadoRepository, AgregadoRepository>();
        services.AddScoped<ILineaProduccionRepository, LineaProduccionRepository>();
        services.AddScoped<ITipoProduccionRepository, TipoProduccionRepository>();
        
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddMediatR(cfg=>  cfg.RegisterServicesFromAssemblyContaining<Program>());
        
        // Registrar validadores
        services.AddValidatorsFromAssemblyContaining<Program>();
        
        // Registrar pipeline de validación
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
    })

    .Build();

host.Run();