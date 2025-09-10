using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Infrastructure.Caching.Common;
using Function.Blending.Opt.Infrastructure.Configuration.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Function.Blending.Opt.Infrastructure.Persistence.Catalog;

public sealed class CachingEstadoCalidadCatalogService(
  IEstadoCalidadCatalogService inner,
  IMemoryCache cache,
  IOptions<EstadoCalidadCacheOptions> opts)
  : CachedManyReader<Guid, EstadoCalidadRef>(
      cache: cache,
      enabled: opts.Value.Enabled,
      cacheNulls: opts.Value.CacheNulls,
      ttlSeconds: opts.Value.TtlSeconds,
      prefix: "estado-calidad"),
    IEstadoCalidadCatalogService
{
  public Task<EstadoCalidadRef?> GetByIdAsync(Guid id, CancellationToken ct)
    => GetSingleAsync(id, c => inner.GetByIdAsync(id, c), ct);

  public Task<IDictionary<Guid, EstadoCalidadRef>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
    => GetManyOrCreateAsync(ids, (ks, c) => inner.GetByIdsAsync(ks, c), ct);
}
