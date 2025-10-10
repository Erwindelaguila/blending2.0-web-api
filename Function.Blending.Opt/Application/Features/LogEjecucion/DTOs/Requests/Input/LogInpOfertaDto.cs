namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

public sealed record LogInpOfertaDto(
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
  string? UbicacionAlmacen = null
)
{
  public IReadOnlyList<LogInpOfeParametroDto>? Parametros { get; init; }
  public IReadOnlyList<LogInpOfeOtrosDto>? Otros { get; init; }
}
