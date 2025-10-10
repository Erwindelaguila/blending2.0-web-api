using AutoMapper;
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
    Vo.LogInpDemanda? demanda,
    IReadOnlyList<Vo.LogInpOferta>? oferta,
    Guid creadoPorId,
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

    // DEMANDA (1:1) + hijos
    if (demanda is not null)
    {
      var rowDemanda = mapper.Map<Ef.LogInpDemanda>(demanda, opt => opt.UseExecutionId(ejecucionId));
      db.Set<Ef.LogInpDemanda>().Add(rowDemanda);
      await db.SaveChangesAsync(ct);

      if (demanda.Parametros is { Count: > 0 })
      {
        var normalized = demanda.Parametros
          .GroupBy(p => p.CodigoParametro)
          .Select(g => g.Last())
          .ToList();

        var hijos = mapper.Map<List<Ef.LogInpDemParametro>>(normalized, opt => opt.UseParentId(rowDemanda.Id));
        db.Set<Ef.LogInpDemParametro>().AddRange(hijos);
      }

      await db.SaveChangesAsync(ct);
    }

    // OFERTA (1:1) + hijos
    if (oferta is not null)
    {
      if (oferta is { Count: > 0 })
      {
        var efOferta = mapper.Map<List<Ef.LogInpOferta>>(oferta, opt => opt.UseExecutionId(ejecucionId).UseUserId(creadoPorId));
        db.Set<Ef.LogInpOferta>().AddRange(efOferta);
        await db.SaveChangesAsync(ct); // genera Ids

        for (int i = 0; i < oferta.Count; i++)
        {
          var src = oferta[i];
          if (src.Parametros is not { Count: > 0 }) continue;

          var parentId = efOferta[i].Id;
          var efParametros = mapper.Map<List<Ef.LogInpOfeParametro>>(src.Parametros, opt => opt.UseParentId(parentId));
          var efOtros = mapper.Map<List<Ef.LogInpOfeOtros>>(src.Otros, opt => opt.UseParentId(parentId));

          db.Set<Ef.LogInpOfeParametro>().AddRange(efParametros);
          db.Set<Ef.LogInpOfeOtros>().AddRange(efOtros);
        }

        if (db.ChangeTracker.HasChanges())
          await db.SaveChangesAsync(ct);
      }
    }
  }
}
