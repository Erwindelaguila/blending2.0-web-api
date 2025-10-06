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
        Console.WriteLine("🚀 Configurando Functions Web Application...");
        
        // Middleware de autenticación JWT
        builder.UseMiddleware<JwtAuthenticationMiddleware>();
        // Middleware de manejo de errores global
        builder.UseMiddleware<GlobalErrorHandlingMiddleware>();
        
        Console.WriteLine("✅ Middlewares configurados correctamente");
    })
    .ConfigureServices(services =>
    {
        Console.WriteLine("🔧 Configurando servicios...");
        
        // Configuración de logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });


        Console.WriteLine("📚 Registrando MediatR...");
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        Console.WriteLine("🏗️ Registrando servicios de infraestructura...");
        // Registro de servicios de infraestructura
        services.AddSingleton<ITokenConfigurationService, TokenConfigurationService>();
        services.AddScoped<ITokenClaimExtractor, TokenClaimExtractor>();
        services.AddScoped<IAuthorizationHeaderExtractor, AuthorizationHeaderExtractor>();
        services.AddScoped<IHttpResponseService, HttpResponseService>();
        services.AddScoped<ITokenService, AzureAdTokenService>();
        services.AddScoped<IAzureAppConfigService, AzureAppConfigService>();

        Console.WriteLine("⚙️ Registrando servicios de aplicación...");
        // Registro de servicios de aplicación
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IMenuService, MenuService>();


        Console.WriteLine("🌐 Configurando HttpClient...");
        services.AddHttpClient();

        Console.WriteLine("📊 Configurando Application Insights...");
        // Agregar configuración de aplicación
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        Console.WriteLine("✅ Todos los servicios configurados correctamente");
        //Deploy v10.1.0 - Debug logging
    })
    .Build();

Console.WriteLine("🎯 Iniciando Azure Functions Host...");
host.Run();
