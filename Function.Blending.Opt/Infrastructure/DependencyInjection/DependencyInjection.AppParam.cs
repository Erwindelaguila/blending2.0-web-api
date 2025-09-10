using System;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Infrastructure.Configuration.Options;
using Function.Blending.Opt.Infrastructure.Persistence.Repositories;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static void ConfigureAppParamCaching(IServiceCollection services, IConfiguration cfg)
  {
    // ===== Binder PLANO para AppParamCacheOptions =====
    services.AddOptions<AppParamCacheOptions>()
      .Configure<IConfiguration>((opts, config) =>
      {
        opts.Enabled = GetBool(config, AppParamConstants.Cache.Enabled, true);
        opts.DefaultTtlSeconds = GetInt(config, AppParamConstants.Cache.DefaultTtlSeconds, 300);
        opts.CacheNulls = GetBool(config, AppParamConstants.Cache.CacheNulls, false);

        opts.PerKeyTtlSeconds.Clear();
        foreach (var kv in config.AsEnumerable(makePathsRelative: false))
        {
          if (string.IsNullOrWhiteSpace(kv.Key)) continue;
          const string prefix = AppParamConstants.Cache.PrefixPerKeyTtlSeconds;
          if (!kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;

          var paramKey = kv.Key[prefix.Length..];
          if (string.IsNullOrWhiteSpace(paramKey)) continue;

          if (int.TryParse(kv.Value, out var ttl) && ttl > 0 && !opts.PerKeyTtlSeconds.ContainsKey(paramKey))
            opts.PerKeyTtlSeconds[paramKey] = ttl;
        }
      });

    // Repo base + decorador caché
    services.AddScoped<AppParamRepository>();
    services.AddScoped<IAppParamRepository, CachingAppParamRepository>();
  }
}
