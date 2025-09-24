using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Reflection;
using Function.Blending.Auth.Application.Interfaces.Services;
using Function.Blending.Auth.Infrastructure.Services;
using Function.Blending.Auth.Infrastructure.Middleware;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        // Middleware de autenticación JWT
        builder.UseMiddleware<JwtAuthenticationMiddleware>();
        // Middleware de manejo de errores global
        builder.UseMiddleware<GlobalErrorHandlingMiddleware>();
    })
    .ConfigureServices(services =>
    {
        // Configuración de logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });


        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Registro de servicios de infraestructura
        services.AddSingleton<ITokenConfigurationService, TokenConfigurationService>();
        services.AddScoped<ITokenClaimExtractor, TokenClaimExtractor>();
        services.AddScoped<IAuthorizationHeaderExtractor, AuthorizationHeaderExtractor>();
        services.AddScoped<IHttpResponseService, HttpResponseService>();
        services.AddScoped<ITokenService, BasicTokenService>();
        services.AddScoped<IAzureAppConfigService, AzureAppConfigService>();
        
        // Registro de servicios de aplicación
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IMenuService, MenuService>();


        services.AddHttpClient();

        // Agregar configuración de aplicación
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
