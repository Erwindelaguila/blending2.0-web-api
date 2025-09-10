using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Infrastructure.Persistence.Mappings;

using Ef = Function.Blending.Opt.Infrastructure.Persistence.Models;
using Vo = Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories.Internals;

internal static class LogisticaInputInserter
{
  public static async Task InsertAsync(
    BlendingDbContext db,
    IMapper mapper,
    Guid ejecucionId,
    Vo.LogInpInfo? info,
    Vo.LogInpFiltro? filtro,
    Vo.LogInpOferta? oferta,
    CancellationToken ct)
  {
    // INFO (1:1)
    if (info is not null)
    {
      var row = mapper.Map<Ef.LogInpInfo>(info, opt => opt.UseExecutionId(ejecucionId));
      db.Set<Ef.LogInpInfo>().Add(row);
      await db.SaveChangesAsync(ct);
    }

    // FILTRO (1:1) + hijos
    if (filtro is not null)
    {
      var rowFiltro = mapper.Map<Ef.LogInpFiltro>(filtro, opt => opt.UseExecutionId(ejecucionId));
      db.Set<Ef.LogInpFiltro>().Add(rowFiltro);
      await db.SaveChangesAsync(ct);

      if (filtro.Capacidades is { Count: > 0 })
      {
        var hijos = mapper.Map<List<Ef.LogInpFilCapacidad>>(filtro.Capacidades, opt => opt.UseParentId(rowFiltro.Id));
        db.Set<Ef.LogInpFilCapacidad>().AddRange(hijos);
      }
      if (filtro.Divisiones is { Count: > 0 })
      {
        var hijos = mapper.Map<List<Ef.LogInpFilDivision>>(filtro.Divisiones, opt => opt.UseParentId(rowFiltro.Id));
        db.Set<Ef.LogInpFilDivision>().AddRange(hijos);
      }
      if (filtro.Emparejamientos is { Count: > 0 })
      {
        var hijos = mapper.Map<List<Ef.LogInpFilEmparejamiento>>(filtro.Emparejamientos, opt => opt.UseParentId(rowFiltro.Id));
        db.Set<Ef.LogInpFilEmparejamiento>().AddRange(hijos);
      }

      await db.SaveChangesAsync(ct);
    }

    // OFERTA (1:1) + hijos
    if (oferta is not null)
    {
      var rowOferta = mapper.Map<Ef.LogInpOferta>(oferta, opt => opt.UseExecutionId(ejecucionId));
      db.Set<Ef.LogInpOferta>().Add(rowOferta);
      await db.SaveChangesAsync(ct);

      if (oferta.Parametros is { Count: > 0 })
      {
        var normalized = oferta.Parametros
          .GroupBy(p => p.CodigoParametro)
          .Select(g => g.Last())
          .ToList();

        var hijos = mapper.Map<List<Ef.LogInpOfeParametro>>(normalized, opt => opt.UseParentId(rowOferta.Id));
        db.Set<Ef.LogInpOfeParametro>().AddRange(hijos);
      }

      await db.SaveChangesAsync(ct);
    }
  }
}
