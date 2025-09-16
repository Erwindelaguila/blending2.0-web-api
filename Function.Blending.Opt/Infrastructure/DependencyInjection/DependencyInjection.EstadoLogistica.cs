using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Infrastructure.Configuration.Options;
using Function.Blending.Opt.Infrastructure.Services.Catalog;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static void ConfigureEstadoLogistica(IServiceCollection services, IConfiguration cfg)
  {
    services.AddScoped<IEstadoLogisticaCatalogService, EstadoLogisticaCatalogService>();

    // ===== Options (binding plano) =====
    services.AddOptions<EstadoLogisticaOptions>()
      .Configure<IConfiguration>((opts, config) =>
      {
        var raw = config[ConfigurationKeys.Catalog.LogisticExecutionStatusId];
        opts.EstadoTableId = Guid.TryParse(raw, out var g) ? g : null;

        opts.ExposeColor = !bool.TryParse(
          config[ConfigurationKeys.Catalog.LogisticExecutionStatusProp.ExposeColor], out var expose) || expose;

        var propClave = config[ConfigurationKeys.Catalog.LogisticExecutionStatusProp.ColorClave];
        if (!string.IsNullOrWhiteSpace(propClave))
          opts.ColorPropClave = propClave!;
      });

    services.AddOptions<EstadoLogisticaCacheOptions>()
      .Configure<IConfiguration>((opts, config) =>
      {
        opts.Enabled = GetBool(config, ConfigurationKeys.Catalog.LogisticExecutionStatusCache.Enabled, true);
        opts.TtlSeconds = GetInt(config, ConfigurationKeys.Catalog.LogisticExecutionStatusCache.TtlSeconds, 300);
        opts.CacheNulls = GetBool(config, ConfigurationKeys.Catalog.LogisticExecutionStatusCache.CacheNulls, false);
      });

    // Decorador de caché
    services.AddScoped<IEstadoLogisticaCatalogService>(sp =>
    {
      var inner = ActivatorUtilities.CreateInstance<EstadoLogisticaCatalogService>(sp);
      var cache = sp.GetRequiredService<IMemoryCache>();
      var opts = sp.GetRequiredService<IOptions<EstadoLogisticaCacheOptions>>();
      return new CachingEstadoLogisticaCatalogService(inner, cache, opts);
    });
  }
}
