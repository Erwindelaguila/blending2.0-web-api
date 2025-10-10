using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    if (ids.Length == 0) return [];

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
    if (ids.Length == 0) return [];

    var query =
      from v in db.Set<AuxValue>().AsNoTracking()
      join p in db.Set<AuxProp>().AsNoTracking() on v.PropId equals p.Id
      where ids.Contains(v.RowId) && p.Clave == propClave
      select new { v.RowId, v.Valor };

    return await query.ToDictionaryAsync(x => x.RowId, x => x.Valor, ct);
  }, ct);

  // =========================================================================
  // NUEVOS MÉTODOS
  // =========================================================================

  /// <summary>
  /// Devuelve Id, Clave, Nombre del row y los valores de las propiedades solicitadas (propClave → valor).
  /// Si alguna prop no existe para el row, se incluye con valor null.
  /// </summary>
  public Task<AuxRowWithPropsSnapshot?> GetRowWithPropsAsync(
    Guid rowId,
    IEnumerable<string> propClaves,
    CancellationToken ct) => WithDbAsync(async db =>
    {
      // Normalizar propClaves (case-insensitive)
      var keys = (propClaves ?? [])
                 .Where(k => !string.IsNullOrWhiteSpace(k))
                 .Select(k => k.Trim())
                 .Distinct(StringComparer.OrdinalIgnoreCase)
                 .ToArray();

      // Header del row
      var row = await db.Set<AuxRow>()
                        .AsNoTracking()
                        .Where(r => r.Id == rowId)
                        .Select(r => new { r.Id, r.TableId, r.Clave, r.Nombre })
                        .FirstOrDefaultAsync(ct);

      if (row is null) return null;

      // Si no se pidieron props, devolvemos sólo header con diccionario vacío
      if (keys.Length == 0)
      {
        return new AuxRowWithPropsSnapshot
        {
          Id = row.Id,
          TableId = row.TableId,
          Clave = row.Clave,
          Nombre = row.Nombre,
          Props = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        };
      }

      // Traer valores para las propClaves solicitadas
      var values = await (
        from v in db.Set<AuxValue>().AsNoTracking()
        join p in db.Set<AuxProp>().AsNoTracking() on v.PropId equals p.Id
        where v.RowId == rowId && keys.Contains(p.Clave)
        select new { p.Clave, v.Valor }
      ).ToListAsync(ct);

      // Construir diccionario-Clave → Valor para todas las keys pedidas (faltantes con null)
      var map = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
      foreach (var k in keys) map[k] = null; // inicializa con null
      foreach (var it in values) map[it.Clave] = it.Valor;

      return new AuxRowWithPropsSnapshot
      {
        Id = row.Id,
        TableId = row.TableId,
        Clave = row.Clave,
        Nombre = row.Nombre,
        Props = map
      };
    }, ct);

  /// <summary>
  /// Devuelve múltiples rows (Id, Clave, Nombre) con los valores de las propiedades solicitadas.
  /// Para cada row, se incluyen todas las propClaves pedidas; si no existe el valor, va null.
  /// </summary>
  public Task<IReadOnlyList<AuxRowWithPropsSnapshot>> GetRowsWithPropsAsync(
    IEnumerable<Guid> rowIds,
    IEnumerable<string> propClaves,
    CancellationToken ct) => WithDbAsync(async db =>
    {
      var ids = (rowIds ?? []).Distinct().ToArray();
      if (ids.Length == 0) return [];

      var keys = (propClaves ?? [])
                 .Where(k => !string.IsNullOrWhiteSpace(k))
                 .Select(k => k.Trim())
                 .Distinct(StringComparer.OrdinalIgnoreCase)
                 .ToArray();

      // Headers de rows
      var rows = await db.Set<AuxRow>()
                         .AsNoTracking()
                         .Where(r => ids.Contains(r.Id))
                         .Select(r => new { r.Id, r.TableId, r.Clave, r.Nombre })
                         .ToListAsync(ct);

      if (rows.Count == 0) return [];

      // Si no hay props solicitadas, devolver sólo header
      if (keys.Length == 0)
      {
        return rows.Select(r => new AuxRowWithPropsSnapshot
        {
          Id = r.Id,
          TableId = r.TableId,
          Clave = r.Clave,
          Nombre = r.Nombre,
          Props = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        }).ToList();
      }

      // Valores para {rowIds} x {propClaves}
      var values = await (
        from v in db.Set<AuxValue>().AsNoTracking()
        join p in db.Set<AuxProp>().AsNoTracking() on v.PropId equals p.Id
        where ids.Contains(v.RowId) && keys.Contains(p.Clave)
        select new { v.RowId, p.Clave, v.Valor }
      ).ToListAsync(ct);

      // Agrupar por RowId → (Clave → Valor)
      var byRow = values
        .GroupBy(x => x.RowId)
        .ToDictionary(
          g => g.Key,
          g =>
          {
            var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var k in keys) dict[k] = null; // inicializa todas las claves pedidas
            foreach (var it in g) dict[it.Clave] = it.Valor;
            return (IReadOnlyDictionary<string, string?>)dict;
          });

      // Proyección final
      var result = rows.Select(r =>
      {
        var props = byRow.TryGetValue(r.Id, out var m)
                   ? m
                   : new Dictionary<string, string?>(keys.Select(k => new KeyValuePair<string, string?>(k, null)), StringComparer.OrdinalIgnoreCase);

        return new AuxRowWithPropsSnapshot
        {
          Id = r.Id,
          TableId = r.TableId,
          Clave = r.Clave,
          Nombre = r.Nombre,
          Props = props
        };
      }).ToList();

      return (IReadOnlyList<AuxRowWithPropsSnapshot>)result;
    }, ct);
}
