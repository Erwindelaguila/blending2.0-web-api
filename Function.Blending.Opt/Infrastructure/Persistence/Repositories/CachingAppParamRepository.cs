using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Infrastructure.Caching.Common;
using Function.Blending.Opt.Infrastructure.Caching.Policies;
using Function.Blending.Opt.Infrastructure.Configuration.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories;

/// <summary>
/// Decorador con IMemoryCache para AppParam (cache-aside con TTL por clave).
/// </summary>
public sealed class CachingAppParamRepository(
  AppParamRepository inner,
  IMemoryCache cache,
  IOptions<AppParamCacheOptions> opts)
  : CachedSingleReader<string, string?>(
      cache: cache,
      enabled: opts.Value.Enabled,
      cacheNulls: opts.Value.CacheNulls,
      ttlResolver: new StringMapTtlPolicy(
          perKey: opts.Value.PerKeyTtlSeconds,
          defaultTtlSeconds: opts.Value.DefaultTtlSeconds
        ).Resolve,
      prefix: "appparam"),
    IAppParamRepository
{
  public Task<string?> GetValueAsync(string key, CancellationToken ct)
    => string.IsNullOrWhiteSpace(key)
       ? Task.FromResult<string?>(null)
       : GetOrCreateAsync(key, c => inner.GetValueAsync(key, c), ct);
}
