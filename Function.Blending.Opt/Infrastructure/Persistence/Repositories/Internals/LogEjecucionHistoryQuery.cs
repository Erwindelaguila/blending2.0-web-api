using System;
using System.Linq;

// Aliases
using E = Function.Blending.Opt.Infrastructure.Persistence.Models;

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories.Internals;

internal static class LogEjecucionHistoryQuery
{
  public static IQueryable<E.LogEjecucion> ApplyFilters(
    IQueryable<E.LogEjecucion> q,
    bool? confirmado,
    DateTime? creadoDelUtc,
    DateTime? creadoAlUtc,
    Guid? estadoId,
    Guid? plantaId,
    string? codigo)
  {
    if (creadoDelUtc.HasValue) q = q.Where(x => x.CreadoEl >= creadoDelUtc.Value);
    if (creadoAlUtc.HasValue) q = q.Where(x => x.CreadoEl <= creadoAlUtc.Value);
    if (estadoId.HasValue) q = q.Where(x => x.EstadoId == estadoId.Value);
    if (plantaId.HasValue) q = q.Where(x => x.PlantaId == plantaId.Value);

    if (!string.IsNullOrWhiteSpace(codigo))
    {
      var needle = codigo.Trim().ToUpperInvariant();
      q = q.Where(x => x.Codigo != null && x.Codigo.ToUpper().Contains(needle));
    }

    if (confirmado.HasValue)
    {
      q = q.Where(x => x.Confirmado == confirmado.Value);
    }

    return q;
  }

  public static IOrderedQueryable<E.LogEjecucion> ApplyOrdering(
    IQueryable<E.LogEjecucion> q,
    string? sortBy,
    string? sortDir)
  {
    var by = (sortBy ?? "creadoEl").Trim().ToLowerInvariant();
    var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);

    return by switch
    {
      "confirmado" => desc ? q.OrderByDescending(x => x.Confirmado) : q.OrderBy(x => x.Confirmado),
      "codigo" => desc ? q.OrderByDescending(x => x.Codigo) : q.OrderBy(x => x.Codigo),
      "estadoid" => desc ? q.OrderByDescending(x => x.EstadoId) : q.OrderBy(x => x.EstadoId),
      "plantaid" => desc ? q.OrderByDescending(x => x.PlantaId) : q.OrderBy(x => x.PlantaId),
      _ => desc ? q.OrderByDescending(x => x.CreadoEl) : q.OrderBy(x => x.CreadoEl),
    };
  }
}
