using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  /// <summary>
  /// Punto de entrada: registra toda la infraestructura (DB, repos, cachés, catálogos, reloj, etc).
  /// </summary>
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
  {
    RegisterDbContext(services, cfg);
    RegisterDomainRepositories(services);
    RegisterDomainServices(services);

    RegisterCoreInfra(services);                  // IMemoryCache, Clock
    ConfigureAppParamCaching(services);           // AppParam: options + repo cacheado
    ConfigureSysParamCaching(services);           // SysParam: options + repo cacheado + servicio high-level

    ConfigureEstadoCalidad(services, cfg);        // Catálogo Estados Calidad (+ caché)
    ConfigureEstadoLogistica(services, cfg);      // Catálogo Estados Logística (+ caché)

    RegisterTimeAndCodes(services);               // Zona horaria y generadores de código

    ConfigureExternalClients(services, cfg);      // Clientes externos (HTTP)
    ConfigureSysLog(services, cfg);               // Logging a DDBB

    ConfigureTime(services, cfg);                 // Time Zone Service

    return services;
  }
}
