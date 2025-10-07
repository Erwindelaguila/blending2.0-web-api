namespace Function.Blending.Opt.Domain.ValueObjects;
public sealed record LogInpDemanda(
  Guid? Id = default,
  Guid? EjecucionId = default,
  int? Posicion = null,
  string? Material = null,
  string? Descripcion = null,
  decimal? CantidadAsignadaVenta = null,
  string? UnidadMedidaVenta = null,
  decimal? CantidadAsignadaAlmacen = null,
  string? UnidadMedidaAlmacen = null,
  decimal? Tolerancia = null,
  IReadOnlyList<LogInpDemParametro>? Parametros = null
);
