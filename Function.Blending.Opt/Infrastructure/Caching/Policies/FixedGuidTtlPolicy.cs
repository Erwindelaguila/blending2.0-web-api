using System;
using Function.Blending.Opt.Infrastructure.Caching.Policies;

namespace Function.Blending.Opt.Infrastructure.Caching.Policies;

/// <summary>
/// TTL fijo para claves Guid (Estados, etc.). Crea instancias distintas si necesitas TTL distintos.
/// </summary>
public sealed class FixedGuidTtlPolicy(int ttlSeconds) : ITtlPolicy<Guid>
{
  private readonly int _ttl = ttlSeconds > 0 ? ttlSeconds : 60;

  public int Resolve(Guid _) => _ttl;
}
