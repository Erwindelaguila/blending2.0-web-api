using System;
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
/// Decorador con IMemoryCache para SysParam (cache-aside con TTL por clave).
/// </summary>
public sealed class CachingSysParamRepository(
  SysParamRepository inner,
  IMemoryCache cache,
  IOptions<SysParamCacheOptions> opts)
  : CachedSingleReader<string, Guid?>(
      cache: cache,
      enabled: opts.Value.Enabled,
      cacheNulls: opts.Value.CacheNulls,
      ttlResolver: new StringMapTtlPolicy(
        perKey: opts.Value.PerKeyTtlSeconds,
        defaultTtlSeconds: opts.Value.DefaultTtlSeconds
      ).Resolve,
      prefix: "sysparam"),
    ISysParamRepository
{
  public Task<Guid?> GetIdAsync(string key, CancellationToken ct)
    => string.IsNullOrWhiteSpace(key)
       ? Task.FromResult<Guid?>(null)
       : GetOrCreateAsync(key, c => inner.GetIdAsync(key, c), ct);
}
