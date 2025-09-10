using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Infrastructure.Configuration.Options;
using Microsoft.Extensions.Options;

namespace Function.Blending.Opt.Infrastructure.Persistence.Catalog;

public sealed class EstadoCalidadCatalogService(IAuxCatalogReader aux, IOptions<EstadoCalidadOptions> opt) : IEstadoCalidadCatalogService
{
  private readonly EstadoCalidadOptions _opts = opt.Value;

  public async Task<EstadoCalidadRef?> GetByIdAsync(Guid id, CancellationToken ct)
  {
    var head = await aux.GetRowHeaderAsync(id, ct);
    if (head is null) return null;

    if (_opts.EstadoTableId.HasValue && head.TableId != _opts.EstadoTableId.Value)
      return null;

    string? color = null;
    if (_opts.ExposeColor && !string.IsNullOrWhiteSpace(_opts.ColorPropClave))
      color = await aux.GetRowPropValueAsync(id, _opts.ColorPropClave, ct);

    return new EstadoCalidadRef(head.Id) { Nombre = head.Nombre, Color = color };
  }

  public async Task<IDictionary<Guid, EstadoCalidadRef>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
  {
    var names = await aux.GetRowNamesAsync(ids, ct);
    var dict = new Dictionary<Guid, EstadoCalidadRef>(names.Count);

    if (!_opts.EstadoTableId.HasValue)
    {
      foreach (var kv in names)
        dict[kv.Key] = new EstadoCalidadRef(kv.Key) { Nombre = kv.Value };
      return dict;
    }

    foreach (var kv in names)
    {
      var head = await aux.GetRowHeaderAsync(kv.Key, ct);
      if (head is null) continue;
      if (head.TableId != _opts.EstadoTableId.Value) continue;

      dict[kv.Key] = new EstadoCalidadRef(kv.Key) { Nombre = kv.Value };
    }
    return dict;
  }
}
