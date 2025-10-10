using Function.Blending.Core.Application.Common.Behaviors;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Infrastructure.Mappings;
using Function.Blending.Core.Infrastructure.Persistence;
using Function.Blending.Core.Infrastructure.Persistence.Mappings;
using Function.Blending.Core.Infrastructure.Persistence.Repositories;
using Function.Blending.Core.Functions.Pipeline;
using Function.Blending.Core.Shared.Constants;

using FluentValidation;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        // Simplified JWT authorization middleware
        builder.UseMiddleware<FunctionContextAccessorMiddleware>();
        builder.UseMiddleware<PrincipalResolutionMiddleware>();
        builder.UseMiddleware<AuthorizationMiddleware>();
    })
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
        
        // Memory Cache para optimización de performance!!!!!!
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
            cfg.AddProfile<AuxTablePorfile>();
            cfg.AddProfile<AuxRowPorfile>();
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
        services.AddScoped<IAuxTableRepository, AuxTableRepository>();
        services.AddScoped<IAuxRowRepository, AuxRowRepository>();
        

        
        // SERVICIO DE USUARIO ACTUAL SIMPLIFICADO
        services.AddHttpContextAccessor();
        
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        // === MediatR con Pipeline de Validación ===
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // === FluentValidation ===
        services.AddValidatorsFromAssemblyContaining<Program>();
        
        // === JWT Authorization Services ===
        services.AddSingleton<Function.Blending.Core.Functions.Support.Authorization.IFunctionAttributeReader, 
                              Function.Blending.Core.Functions.Support.Authorization.FunctionAttributeReader>();
        services.AddSingleton<Function.Blending.Core.Functions.Support.Authorization.IAuthorizationService, 
                              Function.Blending.Core.Functions.Support.Authorization.AuthorizationService>();
        services.AddSingleton<Function.Blending.Core.Functions.Support.Security.IPrincipalResolver, 
                              Function.Blending.Core.Functions.Support.Security.PrincipalResolver>();
        services.AddSingleton<Function.Blending.Core.Functions.Support.Execution.IFunctionContextAccessor, 
                              Function.Blending.Core.Functions.Support.Execution.FunctionContextAccessor>();
        services.AddSingleton<Function.Blending.Core.Functions.Support.ProblemDetails.ProblemDetailsFactory>();
        services.AddSingleton<Function.Blending.Core.Functions.Support.ProblemDetails.IProblemDetailsWriter, 
                              Function.Blending.Core.Functions.Support.ProblemDetails.ProblemDetailsWriter>();
        
        // === Configurar opciones de autorización ===
        services.AddOptions<Function.Blending.Core.Functions.Configuration.Options.AuthorizationOptions>()
            .Configure<Microsoft.Extensions.Configuration.IConfiguration>((opts, config) =>
            {
                // DevBypass
                if (bool.TryParse(config[Function.Blending.Core.Shared.Constants.ConfigurationKeys.Auth.DevBypass], out var devBypass))
                    opts.DevBypass = devBypass;
                else
                    opts.DevBypass = false;

                // DevGroups (CSV)
                static string[] SplitCsv(string? csv) =>
                    string.IsNullOrWhiteSpace(csv)
                        ? Array.Empty<string>()
                        : csv.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                opts.DevGroups = SplitCsv(config[Function.Blending.Core.Shared.Constants.ConfigurationKeys.Auth.DevGroups]);

                // Allow:* (CSV) - Load ALL Auth_Allow_* keys dynamically from configuration
                opts.Allow.Clear();
                var allowPrefix = "Auth_Allow_";
                
                // Get all configuration keys that start with "Auth_Allow_"
                var allConfigKeys = Environment.GetEnvironmentVariables()
                    .Cast<System.Collections.DictionaryEntry>()
                    .Select(kvp => kvp.Key.ToString())
                    .Where(key => key.StartsWith(allowPrefix, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                Console.WriteLine($"[DEBUG CONFIG] Found {allConfigKeys.Length} Auth_Allow_* keys in environment");

                foreach (var fullKey in allConfigKeys)
                {
                    var value = config[fullKey];
                    if (string.IsNullOrWhiteSpace(value)) continue;
                    
                    // Extract the scope name by removing the "Auth_Allow_" prefix
                    var scopeName = fullKey.Substring(allowPrefix.Length);
                    var groups = SplitCsv(value);
                    
                    Console.WriteLine($"[DEBUG CONFIG] Loading scope: '{scopeName}' with groups: [{string.Join(", ", groups)}]");
                    
                    if (!opts.Allow.ContainsKey(scopeName))
                        opts.Allow[scopeName] = groups;
                }
                
                Console.WriteLine($"[DEBUG CONFIG] Total scopes loaded: {opts.Allow.Count}");
            });
    })
    .Build();

//Segurity Victor
/*
bool on(string key, bool def = true) => bool.TryParse(builder.Configuration[key], out var b) ? b : def;
builder.Services.AddFunctionsSupport(builder.Configuration);

builder.Services.AddSingleton<IKeyDecoder, KeyDecoder>();

builder.UseFunctionsPipeline(
    enableExceptionHandling: on(ConfigurationKeys.Pipeline.EnableExceptionHandling),
    enableRequestLogging: on(ConfigurationKeys.Pipeline.EnableRequestLogging),
    enableRequestSizeLimit: on(ConfigurationKeys.Pipeline.EnableRequestSizeLimit),
    enableAuthentication: on(ConfigurationKeys.Pipeline.EnableAuthentication)
);
*/


//Deploy v7.0.0

host.Run();
