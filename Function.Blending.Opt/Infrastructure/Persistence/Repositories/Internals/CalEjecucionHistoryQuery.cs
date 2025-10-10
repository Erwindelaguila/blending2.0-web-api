using System.Linq.Expressions;

// Aliases
using E = Function.Blending.Opt.Infrastructure.Persistence.Models;

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories.Internals;

internal static class CalEjecucionHistoryQuery
{
  // Expresión traducible por EF (maneja nulls sin usar '!')
  private static readonly Expression<Func<E.CalEjecucion, string>> EstadoNombreOrEmptyExpr = x => (x.Estado != null ? x.Estado.Nombre : null) ?? "";

  public static IQueryable<E.CalEjecucion> ApplyFilters(
    IQueryable<E.CalEjecucion> q,
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

    return q;
  }

  public static IOrderedQueryable<E.CalEjecucion> ApplyOrdering(IQueryable<E.CalEjecucion> q, string? sortBy, string? sortDir)
  {
    var by = (sortBy ?? "creadoEl").Trim().ToLowerInvariant();
    var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);

    return by switch
    {
      "codigo" => desc ? q.OrderByDescending(x => x.Codigo) : q.OrderBy(x => x.Codigo),
      "estado" => desc ? q.OrderByDescending(EstadoNombreOrEmptyExpr) : q.OrderBy(EstadoNombreOrEmptyExpr),
      "plantaid" => desc ? q.OrderByDescending(x => x.PlantaId) : q.OrderBy(x => x.PlantaId),
      _ => desc ? q.OrderByDescending(x => x.CreadoEl) : q.OrderBy(x => x.CreadoEl),
    };
  }
}
