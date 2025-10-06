namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses.Output;

public sealed class CalOutDetalleDto
{
  public Guid Id { get; init; }

  public Guid EjecucionId { get; init; }

  public string? Grupo { get; init; }

  public string? Ruma { get; init; }

  public decimal? KilosUsados { get; init; }

  public decimal? Cantidad { get; init; }

  public string? Codigo { get; init; }

  public string? DescripcionMaterial { get; init; }

  public string? CentroUbicacion { get; init; }

  public string? AlmacenUbicacion { get; init; }

  public string? FechaContabilizacion { get; init; }

  public string? FechaFabricacion { get; init; }

  public string? NuevaFechaFabricacion { get; init; }

  public bool? Aceptado { get; init; } = false;

  public IReadOnlyList<CalOutDetParametroDto>? Parametros { get; init; }

  public IReadOnlyList<CalOutDetOtrosDto>? Otros { get; init; }
}
