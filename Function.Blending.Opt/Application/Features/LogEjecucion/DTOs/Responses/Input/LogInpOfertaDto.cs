namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input;

public sealed record LogInpOfertaDto
{
  public string Ruma { get; init; } = default!;
  public int Posicion { get; init; }
  public string DescripcionMaterial { get; init; } = default!;
  public string DescripcionCentro { get; init; } = default!;
  public decimal CantidadAsignadaAlmacen { get; init; }
  public string? UnidadMedidaAlmacen { get; init; }
  public string FechaContabilizacion { get; init; } = default!;
  public string FechaFabricacion { get; init; } = default!;
  public decimal CantidadAsignadaVenta { get; init; }
  public string? UnidadMedidaVenta { get; init; }
  public string? FechaAnalisisQuimico { get; init; }
  public string? FechaVencimientoQuimico { get; init; }
  public string? FechaAnalisisMicrobiologico { get; init; }
  public string? FechaVencimientoMicrobiologico { get; init; }
  public string? TipoAlmacen  { get; init; }
  public string? UbicacionAlmacen  { get; init; }
  public IReadOnlyList<LogInpOfeParametroDto>? Parametros { get; init; }
  public IReadOnlyList<LogInpOfeOtrosDto>? Otros { get; init; }
}
