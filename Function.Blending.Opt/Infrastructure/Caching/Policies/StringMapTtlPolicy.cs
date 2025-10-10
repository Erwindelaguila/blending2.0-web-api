using System.Collections.Generic;
using Function.Blending.Opt.Infrastructure.Caching.Policies;

namespace Function.Blending.Opt.Infrastructure.Caching.Policies;

/// <summary>
/// TTL por clave: si la clave está en el mapa, usa ese TTL; si no, usa el default.
/// </summary>
public sealed class StringMapTtlPolicy(
  IReadOnlyDictionary<string, int>? perKey,
  int defaultTtlSeconds,
  IEqualityComparer<string>? comparer = null) : ITtlPolicy<string>
{
  private readonly int _defaultTtl = defaultTtlSeconds > 0 ? defaultTtlSeconds : 60;
  private readonly IEqualityComparer<string> _cmp = comparer ?? StringComparer.Ordinal;

  public int Resolve(string key)
  {
    if (!string.IsNullOrWhiteSpace(key) && perKey is not null)
    {
      // Búsqueda segura con comparador (case sensitive por defecto)
      foreach (var kv in perKey)
      {
        if (_cmp.Equals(kv.Key, key) && kv.Value > 0)
          return kv.Value;
      }
    }
    return _defaultTtl;
  }
}
