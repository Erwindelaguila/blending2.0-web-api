          using FluentValidation;
using Function.Blending.Core.Application.Common.Behaviors;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Products.Commands;
using Function.Blending.Core.Application.Validators;
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
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<ProductoProfile>();
            cfg.AddProfile<PlantaProfile>();
            cfg.AddProfile<CalidadProfile>();
            cfg.AddProfile<ParametroProfile>();
            cfg.AddProfile<AgregadoProfile>();
        });
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IPlantaRepository, PlantaRepository>();
        services.AddScoped<ICalidadRepository, CalidadRepository>();
        services.AddScoped<IParametroRepository, ParametroRepository>();
        services.AddScoped<IAgregadoRepository, AgregadoRepository>();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddMediatR(cfg=>  cfg.RegisterServicesFromAssemblyContaining<Program>());
        
        // Registrar validadores
        services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
        services.AddValidatorsFromAssemblyContaining<Program>();
        
        // Registrar pipeline de validación
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
    })

    .Build();

host.Run();