using System.Collections.Generic;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input
{
  public sealed record LogInpFiltroDto(
    int PesoContenedor,
    bool ActualizarCapacidad,
    string? Parametros,
    bool? HabilitarDivision,
    string? Division,
    decimal? TiempoEspera
  )
  {
    // Colecciones: nombres en plural
    public IReadOnlyList<LogInpFilCapacidadDto>? Capacidades { get; init; }
    public IReadOnlyList<LogInpFilDivisionDto>? Divisiones { get; init; }
    public IReadOnlyList<LogInpFilEmparejamientoDto>? Emparejamientos { get; init; }
  }
}
