using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Infrastructure.Caching.Common;          // <- base genérica
using Function.Blending.Opt.Infrastructure.Configuration.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Function.Blending.Opt.Infrastructure.Persistence.Catalog;

public sealed class CachingEstadoLogisticaCatalogService(
  IEstadoLogisticaCatalogService inner,
  IMemoryCache cache,
  IOptions<EstadoLogisticaCacheOptions> opts)
  : CachedManyReader<Guid, EstadoLogisticaRef>(
      cache: cache,
      enabled: opts.Value.Enabled,
      cacheNulls: opts.Value.CacheNulls,
      ttlSeconds: opts.Value.TtlSeconds,
      prefix: "estado-logistica"),
    IEstadoLogisticaCatalogService
{
  public Task<EstadoLogisticaRef?> GetByIdAsync(Guid id, CancellationToken ct)
    => GetSingleAsync(id, c => inner.GetByIdAsync(id, c), ct);

  public Task<IDictionary<Guid, EstadoLogisticaRef>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
    => GetManyOrCreateAsync(ids, (ks, c) => inner.GetByIdsAsync(ks, c), ct);
}
