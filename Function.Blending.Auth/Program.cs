using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Reflection;
using Function.Blending.Auth.Application.Interfaces.Services;
using Function.Blending.Auth.Infrastructure.Services;

// Trigger deployment - Auth function updated
// Fixed OIDC permissions for Azure deployment

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Configuración de logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Registro de MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // ===== SERVICIOS SIMPLIFICADOS PARA APIM =====
        // Ya no necesitamos validación JWT - APIM lo maneja
        // services.AddSingleton<ITokenConfigurationService, TokenConfigurationService>(); // ❌ ELIMINADO
        // services.AddScoped<ITokenClaimExtractor, TokenClaimExtractor>(); // ❌ ELIMINADO
        // services.AddScoped<ITokenClaimValidator, TokenClaimValidator>(); // ❌ ELIMINADO
        // services.AddScoped<ITokenSignatureValidator, TokenSignatureValidator>(); // ❌ ELIMINADO
        // services.AddScoped<ITokenService, SimpleTokenService>(); // ❌ ELIMINADO
        // services.AddScoped<IAuthorizationHeaderExtractor, AuthorizationHeaderExtractor>(); // ❌ ELIMINADO
        
        // ✅ NUEVOS SERVICIOS PARA HEADERS
        services.AddScoped<IHeaderUserService, HeaderUserService>();
        
        // ✅ SERVICIOS DE CONFIGURACIÓN Y NEGOCIO
        services.AddScoped<IAzureAppConfigService, AzureAppConfigService>();
        services.AddScoped<IHttpResponseService, HttpResponseService>();
        
        // ✅ SERVICIOS DE APLICACIÓN PARA CLEAN ARCHITECTURE
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IMenuService, MenuService>();

        // Configuración de HttpClient para llamadas externas si es necesario
        services.AddHttpClient();

        // Agregar configuración de aplicación
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
// Deploy trigger Tue Sep  2 08:07:31 PDT 2025-v8
