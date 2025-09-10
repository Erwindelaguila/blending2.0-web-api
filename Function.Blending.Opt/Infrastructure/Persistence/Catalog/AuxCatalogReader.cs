using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Models;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Opt.Infrastructure.Persistence.Catalog;

/// <summary>Implementación EF del lector Aux*.</summary>
public sealed class AuxCatalogReader(BlendingDbContext db) : IAuxCatalogReader
{
  public async Task<AuxRowSnapshot?> GetRowHeaderAsync(Guid rowId, CancellationToken ct)
  {
    var row = await db.Set<AuxRow>()
                      .AsNoTracking()
                      .Where(r => r.Id == rowId)
                      .Select(r => new { r.Id, r.TableId, r.Nombre })
                      .FirstOrDefaultAsync(ct);

    if (row is null) return null;

    // Usamos inicializador por propiedades para evitar depender de un ctor específico
    return new AuxRowSnapshot
    {
      Id = row.Id,
      TableId = row.TableId,
      Nombre = row.Nombre
    };
  }

  public async Task<Dictionary<Guid, string>> GetRowNamesAsync(IEnumerable<Guid> rowIds, CancellationToken ct)
  {
    var ids = rowIds.Distinct().ToArray();
    if (ids.Length == 0) return new();

    return await db.Set<AuxRow>()
                   .AsNoTracking()
                   .Where(r => ids.Contains(r.Id))
                   .Select(r => new { r.Id, r.Nombre })
                   .ToDictionaryAsync(x => x.Id, x => x.Nombre, ct);
  }

  public async Task<string?> GetRowPropValueAsync(Guid rowId, string propClave, CancellationToken ct)
  {
    var valor = await (
      from v in db.Set<AuxValue>().AsNoTracking()
      join p in db.Set<AuxProp>().AsNoTracking() on v.PropId equals p.Id
      where v.RowId == rowId && p.Clave == propClave
      select v.Valor
    ).FirstOrDefaultAsync(ct);

    return valor;
  }

  public async Task<Dictionary<Guid, string>> GetRowPropValuesAsync(IEnumerable<Guid> rowIds, string propClave, CancellationToken ct)
  {
    var ids = rowIds.Distinct().ToArray();
    if (ids.Length == 0) return new();

    var query =
      from v in db.Set<AuxValue>().AsNoTracking()
      join p in db.Set<AuxProp>().AsNoTracking() on v.PropId equals p.Id
      where ids.Contains(v.RowId) && p.Clave == propClave
      select new { v.RowId, v.Valor };

    return await query.ToDictionaryAsync(x => x.RowId, x => x.Valor, ct);
  }
}
