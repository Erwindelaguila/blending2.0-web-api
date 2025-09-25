using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Infrastructure.Persistence;
using Function.Blending.Opt.Infrastructure.Persistence.Models;
using Function.Blending.Opt.Infrastructure.Persistence.Support;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Opt.Infrastructure.Services.Catalog;

/// <summary>Implementación EF del lector Aux*.</summary>
public sealed class AuxCatalogReader(IDbContextFactory<BlendingDbContext> dbFactory) : PooledQueryRepository<BlendingDbContext>(dbFactory), IAuxCatalogReader
{
  public Task<AuxRowSnapshot?> GetRowHeaderAsync(Guid rowId, CancellationToken ct) => WithDbAsync(async db =>
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
  }, ct);

  public Task<Dictionary<Guid, string>> GetRowNamesAsync(IEnumerable<Guid> rowIds, CancellationToken ct) => WithDbAsync(async db =>
  {
    var ids = rowIds.Distinct().ToArray();
    if (ids.Length == 0) return new();

    return await db.Set<AuxRow>()
                   .AsNoTracking()
                   .Where(r => ids.Contains(r.Id))
                   .Select(r => new { r.Id, r.Nombre })
                   .ToDictionaryAsync(x => x.Id, x => x.Nombre, ct);
  }, ct);

  public Task<string?> GetRowPropValueAsync(Guid rowId, string propClave, CancellationToken ct) => WithDbAsync(async db =>
  {
    var valor = await (
      from v in db.Set<AuxValue>().AsNoTracking()
      join p in db.Set<AuxProp>().AsNoTracking() on v.PropId equals p.Id
      where v.RowId == rowId && p.Clave == propClave
      select v.Valor
    ).FirstOrDefaultAsync(ct);

    return valor;
  }, ct);

  public Task<Dictionary<Guid, string>> GetRowPropValuesAsync(IEnumerable<Guid> rowIds, string propClave, CancellationToken ct) => WithDbAsync(async db =>
  {
    var ids = rowIds.Distinct().ToArray();
    if (ids.Length == 0) return new();

    var query =
      from v in db.Set<AuxValue>().AsNoTracking()
      join p in db.Set<AuxProp>().AsNoTracking() on v.PropId equals p.Id
      where ids.Contains(v.RowId) && p.Clave == propClave
      select new { v.RowId, v.Valor };

    return await query.ToDictionaryAsync(x => x.RowId, x => x.Valor, ct);
  }, ct);
}
