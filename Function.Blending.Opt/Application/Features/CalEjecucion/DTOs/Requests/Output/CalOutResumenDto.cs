using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

public sealed record CalOutResumenDto
{
  public string? Grupo { get; init; }
  public int? Toneladas { get; init; }

  // En FE/BE trabajaremos en UTC; en EF se mapea a NuevaFechaFabricacion
  public DateTime? NuevaFechaFabricacionUtc { get; init; }

  public string? CodigoCalidadObjetivo { get; init; }
  public string? CodigoCalidadResultante { get; init; }
  public int? ValorInicial { get; init; }
  public int? ValorFinal { get; init; }
  public int? ValorAgregado { get; init; }
  public bool? Aceptado { get; init; }

  public IReadOnlyList<CalOutResParametroDto>? Parametros { get; init; }
}
