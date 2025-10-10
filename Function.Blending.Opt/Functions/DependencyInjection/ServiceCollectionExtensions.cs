using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Functions.Support.Security;
using Function.Blending.Opt.Functions.Support.Security.PrincipalBuilders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Opt.Functions.DependencyInjection;

public static class ServiceCollectionExtensions
{
  /// <summary>
  /// Registra todos los servicios del "capa Functions": auth helpers, ProblemDetailsFactory, resolvers, builders, etc.
  /// Mantiene la misma lógica que ya usabas en Program.cs pero centralizado.
  /// </summary>
  public static IServiceCollection AddFunctions(this IServiceCollection services, IConfiguration cfg)
  {
    // === Core helpers ===
    services.AddSingleton<IFunctionAttributeReader, FunctionAttributeReader>();
    services.AddSingleton<ProblemDetailsFactory>();

    // === Auth / Security ===
    services.AddSingleton<IPrincipalResolver, PrincipalResolver>();
    services.AddSingleton<IPrincipalBuilder, DevBypassPrincipalBuilder>();
    services.AddSingleton<IPrincipalBuilder, EasyAuthPrincipalBuilder>();
    services.AddSingleton<IPrincipalBuilder, LocalHeaderPrincipalBuilder>();
    services.AddSingleton<IPrincipalBuilder, BearerPrincipalBuilder>(); // se activa por flag
    services.AddSingleton<IPrincipalBuilder, HmacPrincipalBuilder>();

    // Si en el futuro agregas Options fuertes (Limits, Catalog, etc.), enlázalas aquí con services.Configure<T>(cfg.GetSection(...))

    return services;
  }
}
