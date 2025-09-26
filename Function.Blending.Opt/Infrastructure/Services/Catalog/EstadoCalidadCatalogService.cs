using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Infrastructure.Configuration.Options;
using Microsoft.Extensions.Options;

namespace Function.Blending.Opt.Infrastructure.Services.Catalog;

public sealed class EstadoCalidadCatalogService(IAuxCatalogReader aux, IOptions<EstadoCalidadOptions> opt) : IEstadoCalidadCatalogService
{
  private readonly EstadoCalidadOptions _opts = opt.Value;

  public async Task<EstadoCalidadSnapshot?> GetByIdAsync(Guid id, CancellationToken ct)
  {
    var head = await aux.GetRowHeaderAsync(id, ct);
    if (head is null) return null;

    if (_opts.EstadoTableId.HasValue && head.TableId != _opts.EstadoTableId.Value)
      return null;

    string? color = null;
    if (_opts.ExposeColor && !string.IsNullOrWhiteSpace(_opts.ColorPropClave))
      color = await aux.GetRowPropValueAsync(id, _opts.ColorPropClave, ct);

    return new EstadoCalidadSnapshot(head.Id) { Nombre = head.Nombre, Color = color };
  }

  public async Task<IDictionary<Guid, EstadoCalidadSnapshot>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
  {
    var names = await aux.GetRowNamesAsync(ids, ct);
    var dict = new Dictionary<Guid, EstadoCalidadSnapshot>(names.Count);

    if (!_opts.EstadoTableId.HasValue)
    {
      foreach (var kv in names)
        dict[kv.Key] = new EstadoCalidadSnapshot(kv.Key) { Nombre = kv.Value };
      return dict;
    }

    foreach (var kv in names)
    {
      var head = await aux.GetRowWithPropsAsync(kv.Key, [_opts.ColorPropClave], ct);
      if (head is null) continue;
      if (head.TableId != _opts.EstadoTableId.Value) continue;

      var color = head.Props.ContainsKey(_opts.ColorPropClave) ? head.Props[_opts.ColorPropClave] : null;
      dict[kv.Key] = new EstadoCalidadSnapshot(kv.Key) { Nombre = kv.Value, Color = color };
    }
    return dict;
  }
  //public async Task<IDictionary<Guid, EstadoCalidadSnapshot>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
  //{
  //  var names = await aux.GetRowNamesAsync(ids, ct);
  //  var dict = new Dictionary<Guid, EstadoCalidadSnapshot>(names.Count);

  //  if (!_opts.EstadoTableId.HasValue)
  //  {
  //    foreach (var kv in names)
  //      dict[kv.Key] = new EstadoCalidadSnapshot(kv.Key) { Nombre = kv.Value };
  //    return dict;
  //  }

  //  foreach (var kv in names)
  //  {
  //    var head = await aux.GetRowHeaderAsync(kv.Key, ct);
  //    if (head is null) continue;
  //    if (head.TableId != _opts.EstadoTableId.Value) continue;

  //    dict[kv.Key] = new EstadoCalidadSnapshot(kv.Key) { Nombre = kv.Value };
  //  }
  //  return dict;
  //}
}
