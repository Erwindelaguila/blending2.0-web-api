using System.Collections.Generic;

namespace Function.Blending.Opt.Domain.ValueObjects;
public sealed record LogInpOferta(
  Guid? Id = default,
  Guid? EjecucionId = default,
  int? Posicion = null,
  string? Material = null,
  string? Descripcion = null,
  int? CantidadAsignadaVenta = null,
  string? UnidadMedidaVenta = null,
  int? CantidadAsignadaAlmacen = null,
  string? UnidadMedidaAlmacen = null,
  decimal? Tolerancia = null,
  IReadOnlyList<LogInpOfeParametro>? Parametros = null
);
