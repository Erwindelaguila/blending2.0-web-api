using System;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Infrastructure.Configuration.Options;
using Function.Blending.Opt.Infrastructure.Services.Catalog;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static void ConfigureEstadoCalidad(IServiceCollection services, IConfiguration cfg)
  {
    // Lector genérico
    services.AddScoped<IAuxCatalogReader, AuxCatalogReader>();
    services.AddScoped<IEstadoCalidadCatalogService, EstadoCalidadCatalogService>();

    // ===== Options (binding plano) =====
    services.AddOptions<EstadoCalidadOptions>()
      .Configure<IConfiguration>((opts, config) =>
      {
        var raw = config[ConfigurationKeys.Catalog.QualityExecutionStatusId];
        opts.EstadoTableId = Guid.TryParse(raw, out var g) ? g : null;

        opts.ExposeColor = !bool.TryParse(
          config[ConfigurationKeys.Catalog.QualityExecutionStatusProp.ExposeColor], out var expose) || expose;

        var propClave = config[ConfigurationKeys.Catalog.QualityExecutionStatusProp.ColorClave];
        if (!string.IsNullOrWhiteSpace(propClave))
          opts.ColorPropClave = propClave!;
      });

    services.AddOptions<EstadoCalidadCacheOptions>()
      .Configure<IConfiguration>((opts, config) =>
      {
        opts.Enabled = GetBool(config, ConfigurationKeys.Catalog.QualityExecutionStatusCache.Enabled, true);
        opts.TtlSeconds = GetInt(config, ConfigurationKeys.Catalog.QualityExecutionStatusCache.TtlSeconds, 300);
        opts.CacheNulls = GetBool(config, ConfigurationKeys.Catalog.QualityExecutionStatusCache.CacheNulls, false);
      });

    // Decorador de caché
    services.AddScoped<IEstadoCalidadCatalogService>(sp =>
    {
      var inner = ActivatorUtilities.CreateInstance<EstadoCalidadCatalogService>(sp);
      var cache = sp.GetRequiredService<IMemoryCache>();
      var opts = sp.GetRequiredService<IOptions<EstadoCalidadCacheOptions>>();
      return new CachingEstadoCalidadCatalogService(inner, cache, opts);
    });
  }
}
