namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input
{
  public sealed record LogInpFiltroDto
  {
    public decimal PesoContenedor { get; init; }
    public bool ActualizarCapacidad { get; init; }
    public string? Parametros { get; init; }
    public bool? HabilitarDivision { get; init; }
    public string? Division { get; init; }
    public decimal? TiempoEspera { get; init; }

    // Colecciones: nombres en plural
    public IReadOnlyList<LogInpFilCapacidadDto>? Capacidades { get; init; }
    public IReadOnlyList<LogInpFilDivisionDto>? Divisiones { get; init; }
    public IReadOnlyList<LogInpFilEmparejamientoDto>? Emparejamientos { get; init; }
  }
}
