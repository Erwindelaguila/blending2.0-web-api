namespace Function.Blending.Opt.Domain.ValueObjects;
public sealed record LogInpOferta(
  Guid? Id = default,
  Guid? EjecucionId = default,
  string? Ruma = null,
  int? Posicion = null,
  string? DescripcionMaterial = null,
  string? DescripcionCentro = null,
  decimal? CantidadAsignadaAlmacen = null,
  string? UnidadMedidaAlmacen = null,
  string? FechaContabilizacion = null,
  string? FechaFabricacion = null,
  decimal? CantidadAsignadaVenta = null,
  string? UnidadMedidaVenta = null,
  string? FechaAnalisisQuimico = null,
  string? FechaVencimientoQuimico = null,
  string? FechaAnalisisMicrobiologico = null,
  string? FechaVencimientoMicrobiologico = null,
  string? TipoAlmacen = null,
  string? UbicacionAlmacen = null,
  IReadOnlyList<LogInpOfeParametro>? Parametros = null,
  IReadOnlyList<LogInpOfeOtros>? Otros = null
);
