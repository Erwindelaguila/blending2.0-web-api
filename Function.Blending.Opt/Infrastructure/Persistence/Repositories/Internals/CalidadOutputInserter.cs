using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Infrastructure.Persistence.Mappings;

// Aliases
using E = Function.Blending.Opt.Infrastructure.Persistence.Models;
using VO = Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories.Internals;

internal static class CalidadOutputInserter
{
  public static async Task InsertResumenesAsync(
    BlendingDbContext db,
    IMapper mapper,
    Guid ejecucionId,
    Guid userId,
    IReadOnlyList<VO.CalOutResumen> resumenes,
    CancellationToken ct)
  {
    var efResumenes = mapper.Map<List<E.CalOutResumen>>(resumenes, opt => opt.UseExecutionId(ejecucionId).UseUserId(userId));

    db.Set<E.CalOutResumen>().AddRange(efResumenes);
    await db.SaveChangesAsync(ct); // genera Ids

    for (int i = 0; i < resumenes.Count; i++)
    {
      var src = resumenes[i];
      if (src.Parametros is not { Count: > 0 }) continue;

      var parentId = efResumenes[i].Id;
      var childs = mapper.Map<List<E.CalOutResParametro>>(src.Parametros, opt => opt.UseParentId(parentId));

      db.Set<E.CalOutResParametro>().AddRange(childs);
    }

    if (db.ChangeTracker.HasChanges())
      await db.SaveChangesAsync(ct);
  }

  public static async Task InsertDetallesAsync(
    BlendingDbContext db,
    IMapper mapper,
    Guid ejecucionId,
    Guid userId,
    IReadOnlyList<VO.CalOutDetalle> detalles,
    CancellationToken ct)
  {
    var efDetalles = mapper.Map<List<E.CalOutDetalle>>(detalles,
      opt => opt.UseExecutionId(ejecucionId).UseUserId(userId));

    db.Set<E.CalOutDetalle>().AddRange(efDetalles);
    await db.SaveChangesAsync(ct); // genera Ids

    for (int i = 0; i < detalles.Count; i++)
    {
      var src = detalles[i];

      if (src.Parametros is { Count: > 0 })
      {
        var parentId = efDetalles[i].Id;
        var childs = mapper.Map<List<E.CalOutDetParametro>>(src.Parametros,
          opt => opt.UseParentId(parentId));
        db.Set<E.CalOutDetParametro>().AddRange(childs);
      }

      if (src.Otros is { Count: > 0 })
      {
        var parentId = efDetalles[i].Id;
        var childs = mapper.Map<List<E.CalOutDetOtros>>(src.Otros,
          opt => opt.UseParentId(parentId));
        db.Set<E.CalOutDetOtros>().AddRange(childs);
      }
    }

    if (db.ChangeTracker.HasChanges())
      await db.SaveChangesAsync(ct);
  }
}
