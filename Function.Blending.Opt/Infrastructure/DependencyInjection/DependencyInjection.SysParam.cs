using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Infrastructure.Configuration;
using Function.Blending.Opt.Infrastructure.Configuration.Options;
using Function.Blending.Opt.Infrastructure.Persistence.Repositories;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static void ConfigureSysParamCaching(IServiceCollection services, IConfiguration cfg)
  {
    // ===== Binder PLANO para SysParamCacheOptions =====
    services.AddOptions<SysParamCacheOptions>()
      .Configure<IConfiguration>((opts, config) =>
      {
        opts.Enabled = GetBool(config, ConfigurationKeys.SysParam.Cache.Enabled, true);
        opts.DefaultTtlSeconds = GetInt(config, ConfigurationKeys.SysParam.Cache.DefaultTtlSeconds, 300);
        opts.CacheNulls = GetBool(config, ConfigurationKeys.SysParam.Cache.CacheNulls, false);

        opts.PerKeyTtlSeconds.Clear();
        foreach (var kv in config.AsEnumerable(makePathsRelative: false))
        {
          const string prefix = ConfigurationKeys.SysParam.Cache.PerKeyTtlPrefix;
          if (kv.Key?.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) != true) continue;

          var paramKey = kv.Key[prefix.Length..];
          if (int.TryParse(kv.Value, out var ttl) && ttl > 0 && !opts.PerKeyTtlSeconds.ContainsKey(paramKey))
            opts.PerKeyTtlSeconds[paramKey] = ttl;
        }
      });

    // ===== Repositorio + Decorador + Servicio high-level =====
    services.AddScoped<SysParamRepository>();                       // inner
    services.AddScoped<ISysParamRepository, CachingSysParamRepository>(); // decorador
    services.AddScoped<ISysParamService, SysParamService>();
  }
}
