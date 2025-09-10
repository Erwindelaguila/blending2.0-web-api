using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Infrastructure.Persistence.Mappings;

// Aliases
using E = Function.Blending.Opt.Infrastructure.Persistence.Models;
using VO = Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories.Internals;

internal static class CalidadInputInserter
{
  public static async Task InsertAsync(
    BlendingDbContext db,
    IMapper mapper,
    Guid ejecucionId,
    VO.CalInpFiltro? filtro,
    IReadOnlyList<VO.CalInpParametro>? parametros,
    CancellationToken ct)
  {
    var hasFiltro = filtro is not null;
    var hasParams = parametros is { Count: > 0 };

    if (hasFiltro)
    {
      var row = mapper.Map<E.CalInpFiltro>(filtro!, opt => opt.UseExecutionId(ejecucionId));
      db.Set<E.CalInpFiltro>().Add(row);
      await db.SaveChangesAsync(ct); // respeta UQ(EjecucionId)
    }

    if (hasParams)
    {
      var normalized = parametros!
        .GroupBy(p => new { p.CalidadId, p.ParametroId })
        .Select(g => g.Last())
        .ToList();

      var rows = mapper.Map<List<E.CalInpParametro>>(normalized, opt => opt.UseExecutionId(ejecucionId));
      db.Set<E.CalInpParametro>().AddRange(rows);
      await db.SaveChangesAsync(ct); // respeta UQ compuesta
    }
  }
}
