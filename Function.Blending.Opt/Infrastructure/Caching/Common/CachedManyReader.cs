using Microsoft.Extensions.Caching.Memory;

namespace Function.Blending.Opt.Infrastructure.Caching.Common;

public abstract class CachedManyReader<TKey, TValue>(
  IMemoryCache cache,
  bool enabled,
  bool cacheNulls,
  int ttlSeconds,
  string prefix) where TKey : notnull
{
  private readonly int _ttlSeconds = Math.Max(1, ttlSeconds);

  protected async Task<IDictionary<TKey, TValue>> GetManyOrCreateAsync(
    IEnumerable<TKey> keys,
    Func<IEnumerable<TKey>, CancellationToken, Task<IDictionary<TKey, TValue>>> fetchMany,
    CancellationToken ct)
  {
    var unique = keys?.Distinct().ToArray() ?? [];
    var result = new Dictionary<TKey, TValue>();
    if (unique.Length == 0) return result;

    if (!enabled) return await fetchMany(unique, ct);

    var missing = new List<TKey>();
    foreach (var k in unique)
    {
      var ck = $"{prefix}:{k}";
      if (cache.TryGetValue(ck, out TValue? hit) && hit is not null)
        result[k] = hit;
      else
        missing.Add(k);
    }

    if (missing.Count > 0)
    {
      var fetched = await fetchMany(missing, ct);
      var ttl = TimeSpan.FromSeconds(_ttlSeconds);

      foreach (var kv in fetched)
      {
        result[kv.Key] = kv.Value;
        cache.Set($"{prefix}:{kv.Key}", kv.Value, ttl);
      }

      if (cacheNulls)
      {
        var still = missing.Where(m => !fetched.ContainsKey(m));
        foreach (var m in still)
          cache.Set($"{prefix}:{m}", default(TValue), ttl);
      }
    }

    return result;
  }

  // atajo útil para “single” con TTL fijo (cuando no hay per-key TTL)
  protected async Task<TValue?> GetSingleAsync(
    TKey key,
    Func<CancellationToken, Task<TValue?>> factory,
    CancellationToken ct)
  {
    var dict = await GetManyOrCreateAsync(
      [key],
      async (ks, c) =>
      {
        var v = await factory(c);
        return v is null ? [] : new Dictionary<TKey, TValue> { [key] = v };
      },
      ct);

    return dict.TryGetValue(key, out var val) ? val : default;
  }
}
