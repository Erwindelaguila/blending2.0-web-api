using AutoMapper;
using Function.Blending.Opt.Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
// Aliases
using E = Function.Blending.Opt.Infrastructure.Persistence.Models;
using VO = Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories.Internals;

internal static class CalidadOutputUpdater
{

  public static async Task SaveChangesAsync(BlendingDbContext db, Guid modificadoPorId, E.CalEjecucion model, CancellationToken ct)
  {
    if (db.ChangeTracker.HasChanges())
    {
      model.ModificadoPorId = modificadoPorId;
      model.ModificadoEl = DateTime.UtcNow;

      await db.SaveChangesAsync(ct);
    }
  }

  public static async Task UpdaterDetallesAsync(BlendingDbContext db, Guid id, Guid modificadoPorId, List<E.CalOutResumen> resumenesYaNoAceptados, List<E.CalOutResumen> nuevosResumenesAceptados, CancellationToken ct)
  {
    var nuevosGruposResumenesAceptados = nuevosResumenesAceptados.Where(g => g.Grupo != null).Select(g => g.Grupo!.ToLower()).ToList();
    var gruposDeResumenesYaNoAceptados = resumenesYaNoAceptados.Where(r => r.Grupo != null).Select(r => r.Grupo!.ToLower()).Distinct().ToList();

    var nuevosDetallesAceptado = await db.Set<E.CalOutDetalle>().Where(x => x.EjecucionId == id && x.Grupo != null && nuevosGruposResumenesAceptados.Contains(x.Grupo!.ToLower())).ToListAsync(ct);
    var detallesYaNoAceptados = await db.Set<E.CalOutDetalle>().Where(x => x.EjecucionId == id && x.Grupo != null && gruposDeResumenesYaNoAceptados.Contains(x.Grupo!.ToLower())).ToListAsync(ct);

    foreach (var d in nuevosDetallesAceptado)
    {
      d.Aceptado = true;
      d.ModificadoPorId = modificadoPorId;
      d.ModificadoEl = DateTime.UtcNow;
    }

    foreach (var d in detallesYaNoAceptados)
    {
      d.Aceptado = false;
      d.ModificadoPorId = modificadoPorId;
      d.ModificadoEl = DateTime.UtcNow;
    }
  }

  public static async Task<(List<E.CalOutResumen> resumenesYaNoAceptados, List<E.CalOutResumen> nuevosResumenesAceptados)> UpdaterResumenesAsync(BlendingDbContext db, Guid id, IReadOnlyList<Guid> grupos, Guid modificadoPorId, CancellationToken ct)
  {
    var resumenes = await db.Set<E.CalOutResumen>().Where(x => x.EjecucionId == id).ToListAsync(ct);
    var resumenesAceptados = resumenes.Where(r => r.Aceptado).ToList();

    var resumenesYaNoAceptados = resumenesAceptados.Where(r => !grupos.Contains(r.Id)).ToList();

    var nuevosResumenesAceptadosId = grupos.Where(g => !resumenesAceptados.Any(r => r.Id == g)).ToList();
    var nuevosResumenesAceptados = resumenes.Where(r => nuevosResumenesAceptadosId.Contains(r.Id)).ToList();

    foreach (var r in resumenesYaNoAceptados)
    {
      r.Aceptado = false;
      r.ModificadoPorId = modificadoPorId;
      r.ModificadoEl = DateTime.UtcNow;
    }

    foreach (var r in nuevosResumenesAceptados)
    {
      r.Aceptado = true;
      r.ModificadoPorId = modificadoPorId;
      r.ModificadoEl = DateTime.UtcNow;
    }

    return (resumenesYaNoAceptados, nuevosResumenesAceptados);
  }
}
