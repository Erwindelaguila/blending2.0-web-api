using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Linq;
using System.Reflection;
using Function.Blending.Auth.Application.Interfaces.Services;
using Function.Blending.Auth.Infrastructure.Services;
using Function.Blending.Auth.Infrastructure.Middleware;

Console.WriteLine("=== INICIO PROGRAMA AZURE FUNCTIONS ===");
Console.WriteLine($"🏗️ Assembly: {Assembly.GetExecutingAssembly().FullName}");
Console.WriteLine($"🔍 Functions Assembly Location: {Assembly.GetExecutingAssembly().Location}");

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        try
        {
            Console.WriteLine("🚀 Configurando Functions Web Application...");
            
            // Middleware de autenticación JWT
            builder.UseMiddleware<JwtAuthenticationMiddleware>();
            // Middleware de manejo de errores global
            builder.UseMiddleware<GlobalErrorHandlingMiddleware>();
            
            Console.WriteLine("✅ Middlewares configurados correctamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR en configuración de middlewares: {ex.Message}");
            throw;
        }
    })
    .ConfigureServices(services =>
    {
        try
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
            var assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine($"🔍 Registrando MediatR desde assembly: {assembly.FullName}");
            cfg.RegisterServicesFromAssembly(assembly);
        });

            Console.WriteLine("🏗️ Registrando servicios de infraestructura...");
            
            // DIAGNÓSTICO: Verificar variables de entorno Azure AD
            var config = services.BuildServiceProvider().GetService<IConfiguration>();
            Console.WriteLine($"🔍 Environment: {config?["Environment"]}");
            Console.WriteLine($"🔍 AzureAd__TenantId: {config?["AzureAd__TenantId"]}");  
            Console.WriteLine($"🔍 AzureAd:TenantId: {config?["AzureAd:TenantId"]}");
            Console.WriteLine($"🔍 AzureAd__Authority: {config?["AzureAd__Authority"]}");
            Console.WriteLine($"🔍 AzureAd:Authority: {config?["AzureAd:Authority"]}");
            
            // Registro de servicios de infraestructura
            services.AddSingleton<ITokenConfigurationService, TokenConfigurationService>();
            services.AddScoped<ITokenClaimExtractor, TokenClaimExtractor>();
            services.AddScoped<IAuthorizationHeaderExtractor, AuthorizationHeaderExtractor>();
            services.AddScoped<IHttpResponseService, HttpResponseService>();
            services.AddScoped<ITokenService, AzureAdTokenService>();
            services.AddScoped<IAzureAppConfigService, AzureAppConfigService>();        Console.WriteLine("⚙️ Registrando servicios de aplicación...");
        // Registro de servicios de aplicación
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IMenuService, MenuService>();


        Console.WriteLine("🌐 Configurando HttpClient...");
        services.AddHttpClient();

        Console.WriteLine("📊 Configurando Application Insights...");
        // Agregar configuración de aplicación
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        Console.WriteLine("🔍 Verificando funciones en el assembly...");
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var functionTypes = types.Where(t => t.GetMethods().Any(m => m.GetCustomAttribute<FunctionAttribute>() != null)).ToArray();
        Console.WriteLine($"📊 Tipos con funciones encontrados: {functionTypes.Length}");
        foreach (var type in functionTypes)
        {
            var functionMethods = type.GetMethods().Where(m => m.GetCustomAttribute<FunctionAttribute>() != null);
            foreach (var method in functionMethods)
            {
                var functionAttr = method.GetCustomAttribute<FunctionAttribute>();
                Console.WriteLine($"⚡ Función encontrada: {type.Name}.{method.Name} -> {functionAttr?.Name}");
            }
        }
            
        Console.WriteLine("✅ Todos los servicios configurados correctamente");
        //Deploy v10.4.0 - FUNCTION DISCOVERY FIX
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR en configuración de servicios: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            throw;
        }
    })
    .Build();

Console.WriteLine("🎯 Iniciando Azure Functions Host...!");
try
{
    host.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERROR CRÍTICO al iniciar host: {ex.Message}");
    Console.WriteLine($"StackTrace: {ex.StackTrace}");
    throw;
}
