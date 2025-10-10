// Aliases
using System.Linq.Expressions;
using E = Function.Blending.Opt.Infrastructure.Persistence.Models;

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories.Internals;

internal static class LogEjecucionHistoryQuery
{
  // Expresión traducible por EF (maneja nulls sin usar '!')
  private static readonly Expression<Func<E.LogEjecucion, string>> EstadoNombreOrEmptyExpr = x => (x.Estado != null ? x.Estado.Nombre : null) ?? "";

  public static IQueryable<E.LogEjecucion> ApplyFilters(
    IQueryable<E.LogEjecucion> q,
    bool? confirmado,
    DateTime? creadoDelUtc,
    DateTime? creadoAlUtc,
    Guid? estadoId,
    string? codigo,
    string? contrato
  )
  {
    if (creadoDelUtc.HasValue) q = q.Where(x => x.CreadoEl >= creadoDelUtc.Value);
    if (creadoAlUtc.HasValue) q = q.Where(x => x.CreadoEl <= creadoAlUtc.Value);
    if (estadoId.HasValue) q = q.Where(x => x.EstadoId == estadoId.Value);

    if (!string.IsNullOrWhiteSpace(codigo))
    {
      var needle = codigo.Trim().ToUpperInvariant();
      q = q.Where(x => x.Codigo != null && x.Codigo.ToUpper().Contains(needle));
    }

    if (!string.IsNullOrWhiteSpace(contrato))
    {
      var needle = contrato.Trim().ToUpperInvariant();
      q = q.Where(x => x.LogInpInfo != null && x.LogInpInfo.Contrato.ToUpper().Contains(needle));
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
      "contrato" => desc ? q.OrderByDescending(x => x.LogInpInfo!.Contrato) : q.OrderBy(x => x.LogInpInfo!.Contrato),
      "estado" => desc ? q.OrderByDescending(EstadoNombreOrEmptyExpr) : q.OrderBy(EstadoNombreOrEmptyExpr),
      _ => desc ? q.OrderByDescending(x => x.CreadoEl) : q.OrderBy(x => x.CreadoEl),
    };
  }
}
