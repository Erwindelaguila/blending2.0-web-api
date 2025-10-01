using System.Collections.Generic;

namespace Function.Blending.Opt.Domain.ValueObjects;

public sealed record LogInpFiltro
{
  public int PesoContenedor { get; init; }
  public bool ActualizarCapacidad { get; init; }
  public string? Parametros { get; init; }
  public bool? HabilitarDivision { get; init; }
  public string? Division { get; set; }
  public decimal? TiempoEspera { get; set; }
  public IReadOnlyList<LogInpFilCapacidad>? Capacidades { get; init; }
  public IReadOnlyList<LogInpFilDivision>? Divisiones { get; init; }
  public IReadOnlyList<LogInpFilEmparejamiento>? Emparejamientos { get; init; }
}