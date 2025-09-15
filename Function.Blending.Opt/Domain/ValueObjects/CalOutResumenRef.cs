namespace Function.Blending.Opt.Domain.ValueObjects;

public sealed record class CalOutResumenRef(Guid Id)
{
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

