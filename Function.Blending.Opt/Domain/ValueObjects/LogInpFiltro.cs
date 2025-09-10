using System.Collections.Generic;

namespace Function.Blending.Opt.Domain.ValueObjects
{
  public sealed record LogInpFiltro(
    int PesoContenedor,
    bool ActualizarCapacidad,
    string? Parametros,
    bool? HabilitarDivision,
    string? Division,
    int? TiempoEspera,
    IReadOnlyList<LogInpFilCapacidad>? Capacidades,
    IReadOnlyList<LogInpFilDivision>? Divisiones,
    IReadOnlyList<LogInpFilEmparejamiento>? Emparejamientos
  );
}
