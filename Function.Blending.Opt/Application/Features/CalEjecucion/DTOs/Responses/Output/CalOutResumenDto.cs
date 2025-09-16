namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Response.Output;

public sealed class CalOutResumenDto
{
  public Guid Id { get; init; }
  public Guid EjecucionId { get; init; }
  public string? Grupo { get; init; }

  public decimal? Toneladas { get; init; }

  public string? NuevaFechaFabricacion { get; init; }

  public string? CodigoCalidadObjetivo { get; init; }

  public string? CodigoCalidadResultante { get; init; }

  public decimal? ValorInicial { get; init; }

  public decimal? ValorFinal { get; init; }

  public decimal? ValorAgregado { get; init; }

  public bool? Aceptado { get; init; } = false;
}
