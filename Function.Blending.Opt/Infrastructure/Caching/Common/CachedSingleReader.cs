using Microsoft.Extensions.Caching.Memory;

namespace Function.Blending.Opt.Infrastructure.Caching.Common;

public abstract class CachedSingleReader<TKey, TValue>(
  IMemoryCache cache,
  bool enabled,
  bool cacheNulls,
  Func<TKey, int> ttlResolver,
  string prefix)
{
  protected async Task<TValue?> GetOrCreateAsync(
    TKey key,
    Func<CancellationToken, Task<TValue?>> factory,
    CancellationToken ct)
  {
    if (!enabled) return await factory(ct);
    if (key is null) return default;

    var ck = $"{prefix}:{key}";
    if (cache.TryGetValue(ck, out TValue? hit)) return hit;

    var value = await factory(ct);
    if (value is null && !cacheNulls) return default;

    var ttl = Math.Max(1, ttlResolver(key));
    cache.Set(ck, value, new MemoryCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttl)
    });
    return value;
  }
}
