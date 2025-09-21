using Function.Blending.Core.Infrastructure.Security.Shared.Constants;
using Function.Blending.Core.Infrastructure.Security.Support.Authorization;
using Function.Blending.Core.Infrastructure.Security.Support.Execution;
using Function.Blending.Core.Infrastructure.Security.Support.ProblemDetails;
using Function.Blending.Core.Infrastructure.Security.Support.Security;
using Function.Blending.Core.Infrastructure.Security.Support.Security.PrincipalBuilders;
using Function.Blending.Upload.Functions.Configuration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Core.Infrastructure.Security.Support.Extensions;

public static class ServiceCollectionExtensions
{
  /// <summary>
  /// Registro de TODOS los servicios de la capa Functions (Support).
  /// Esta es la ÚNICA extensión que debes usar en Program.cs → AddFunctionsSupport(...)
  /// </summary>
  public static IServiceCollection AddFunctionsSupport(this IServiceCollection services, IConfiguration cfg)
  {
    // ===== Problem Details (RFC 7807) =====
    services.AddProblemDetailsFactory();
    services.AddSingleton<IProblemDetailsWriter, ProblemDetailsWriter>();

    // ===== Lectura de atributos/metadata en runtime (NECESARIO para middlewares) =====
    services.AddSingleton<IFunctionAttributeReader, FunctionAttributeReader>();
    
    
    // ===== Seguridad / Auth core =====
    services.AddSingleton<IAuthorizationService, AuthorizationService>();
    services.AddSingleton<ITokenValidationService, JwtTokenValidationService>();       

    // ===== Principal Resolution =====
    services.AddSingleton<IPrincipalResolver, PrincipalResolver>();
    services.AddSingleton<IPrincipalBuilder, DevBypassPrincipalBuilder>();
    services.AddSingleton<IPrincipalBuilder, EasyAuthPrincipalBuilder>();
    services.AddSingleton<IPrincipalBuilder, LocalHeaderPrincipalBuilder>();
    services.AddSingleton<IPrincipalBuilder, BearerPrincipalBuilder>();

    // ===== Execution Context (para IRequestContext) =====
    services.AddSingleton<IFunctionContextAccessor, FunctionContextAccessor>();
    services.AddSingleton<IRequestContext, RequestContext>();

    // ===== Options binding =====
    // AuthorizationOptions: mezcla de flags + CSV (Auth_Allow:*) desde config
    services.AddOptions<AuthorizationOptions>()
      .Configure<IConfiguration>((opts, config) =>
      {
        // DevBypass
        if (bool.TryParse(config[ConfigurationKeys.Auth.DevBypass], out var devBypass))
          opts.DevBypass = devBypass;
        else
          opts.DevBypass = false;

        // DevGroups (CSV)
        static string[] SplitCsv(string? csv) =>
          string.IsNullOrWhiteSpace(csv)
            ? Array.Empty<string>()
            : csv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        opts.DevGroups = SplitCsv(config[ConfigurationKeys.Auth.DevGroups]);

        // Allow:* (CSV)
        opts.Allow.Clear();
        foreach (var kv in config.AsEnumerable(makePathsRelative: false))
        {
          if (string.IsNullOrWhiteSpace(kv.Key)) continue;
          if (!kv.Key.StartsWith($"{ConfigurationKeys.Auth.AllowSection}_", StringComparison.OrdinalIgnoreCase)) continue;

          var groups = SplitCsv(kv.Value);
          if (!opts.Allow.ContainsKey(kv.Key))
            opts.Allow[kv.Key] = groups;
        }
      });


    return services;
  }
}
