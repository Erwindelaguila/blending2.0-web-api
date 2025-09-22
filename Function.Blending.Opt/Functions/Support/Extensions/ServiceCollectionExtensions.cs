using Function.Blending.Opt.Functions.Configuration.Options;
using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Functions.Support.Security;
using Function.Blending.Opt.Functions.Support.Security.PrincipalBuilders;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Opt.Functions.Support.Extensions;

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
    services.AddSingleton<ITokenValidationService, JwtTokenValidationService>();          // JWT
    services.AddSingleton<IWebhookSignatureValidator, HmacWebhookSignatureValidator>();   // HMAC (webhooks)

    // ===== Principal Resolution =====
    services.AddSingleton<IPrincipalResolver, PrincipalResolver>();
    services.AddSingleton<IPrincipalBuilder, DevBypassPrincipalBuilder>();
    services.AddSingleton<IPrincipalBuilder, EasyAuthPrincipalBuilder>();
    services.AddSingleton<IPrincipalBuilder, LocalHeaderPrincipalBuilder>();
    services.AddSingleton<IPrincipalBuilder, BearerPrincipalBuilder>();
    services.AddSingleton<IPrincipalBuilder, HmacPrincipalBuilder>();

    // ===== Execution Context (para IRequestContext) =====
    services.AddSingleton<Execution.IFunctionContextAccessor, Execution.FunctionContextAccessor>();
    services.AddSingleton<Execution.IRequestContext, Execution.RequestContext>();

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
            : csv.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

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

    // Tamaño máximo de request para WebHooks y otros (bytes)
    services.Configure<RequestSizeOptions>(o =>
    {
      o.MaxBytes = long.TryParse(cfg[ConfigurationKeys.Limits.WebhookMaxBytes], out var v)
        ? v
        : 2 * 1024 * 1024;
    });

    return services;
  }
}
