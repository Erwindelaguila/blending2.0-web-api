using FluentValidation;
using Function.Blending.Core.Application.Common.Behaviors;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Infrastructure.Services;
using Function.Blending.Core.Infrastructure.Mappings;
using Function.Blending.Core.Infrastructure.Persistence;
using Function.Blending.Core.Infrastructure.Persistence.Mappings;
using Function.Blending.Core.Infrastructure.Persistence.Repositories;
// Trigger deployment - Core function updated
// Fixed OIDC permissions for Azure deployment
// ❌ YA NO NECESARIOS - Function.Auth ya no proporciona servicios de JWT
// using Function.Blending.Auth.Application.Interfaces.Services;
// using Function.Blending.Auth.Infrastructure.Services;
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
            cfg.AddProfile<CalidadParametroProfile>();
            cfg.AddProfile<AppParamProfile>();
            // Removed GraphProfile reference to prevent build error
        });
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IPlantaRepository, PlantaRepository>();
        services.AddScoped<ICalidadRepository, CalidadRepository>();
        services.AddScoped<IParametroRepository, ParametroRepository>();
        services.AddScoped<IAgregadoRepository, AgregadoRepository>();
        services.AddScoped<ILineaProduccionRepository, LineaProduccionRepository>();
        services.AddScoped<ITipoProduccionRepository, TipoProduccionRepository>();
        services.AddScoped<ICalidadParametroRepository, CalidadParametroRepository>();
        services.AddScoped<IAppParamRepository, AppParamRepository>();
        
        // === Servicios de Autenticación (Reutilizando de Function.Blending.Auth) ===
        // Servicios compartidos - siguiendo principio DRY
        // ===== SERVICIOS SIMPLIFICADOS PARA APIM =====
        // Ya no necesitamos validación JWT - APIM/Gateway lo maneja
        // services.AddScoped<IAuthorizationHeaderExtractor, AuthorizationHeaderExtractor>(); // ❌ ELIMINADO
        // services.AddScoped<ITokenClaimExtractor, TokenClaimExtractor>(); // ❌ ELIMINADO
        // services.AddScoped<ITokenClaimValidator, TokenClaimValidator>(); // ❌ ELIMINADO
        // services.AddScoped<ITokenConfigurationService, TokenConfigurationService>(); // ❌ ELIMINADO
        // services.AddScoped<ITokenSignatureValidator, TokenSignatureValidator>(); // ❌ ELIMINADO
        
        // ✅ SERVICIO DE USUARIO ACTUAL SIMPLIFICADO
        // Servicios de autorización y auditoría
        services.AddHttpContextAccessor(); // ← AGREGAMOS ESTO
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IAuditService, AuditService>();
        
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddMediatR(cfg=>  cfg.RegisterServicesFromAssemblyContaining<Program>());
        
        // Registrar validadores
        services.AddValidatorsFromAssemblyContaining<Program>();
        
        // Registrar pipeline behaviors - Clean Architecture
        // Orden importante: Validación → Autorización → Auditoría
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
        
    })
    .Build();

host.Run();
// Test deployment Tue Sep  2 07:51:46 PDT 2026
// Deploy trigger Tue Sep  2 08:07:04 PDT 2027
