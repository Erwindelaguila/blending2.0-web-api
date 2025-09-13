using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Reflection;
using Function.Blending.Auth.Application.Interfaces.Services;
using Function.Blending.Auth.Infrastructure.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Configuración de logging!!
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

        // Registro de servicios de infraestructura
        services.AddSingleton<ITokenConfigurationService, TokenConfigurationService>();
        services.AddScoped<ITokenClaimExtractor, TokenClaimExtractor>();
        services.AddScoped<ITokenClaimValidator, TokenClaimValidator>();
        services.AddScoped<ITokenSignatureValidator, TokenSignatureValidator>();
        services.AddScoped<ITokenService, SimpleTokenService>();
        services.AddScoped<IAzureAppConfigService, AzureAppConfigService>();
        services.AddScoped<IAuthorizationHeaderExtractor, AuthorizationHeaderExtractor>();
        services.AddScoped<IHttpResponseService, HttpResponseService>();
        
        // Registro de servicios de aplicación para Clean Architecture
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
