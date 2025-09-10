using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

public sealed record CalOutDetalleDto
{
  public string? Grupo { get; init; }
  public string? Ruma { get; init; }

  public int? KilosUsados { get; init; }
  public int? Cantidad { get; init; }

  public string? Codigo { get; init; }
  public string? DescripcionMaterial { get; init; }
  public string? CentroUbicacion { get; init; }
  public string? AlmacenUbicacion { get; init; }
  public string? FechaContabilizacion { get; init; }

  // En FE/BE trabajaremos en UTC; en EF se mapea a NuevaFechaFabricacion
  public DateTime? NuevaFechaFabricacionUtc { get; init; }
  public bool? Aceptado { get; init; }

  public IReadOnlyList<CalOutDetParametroDto>? Parametros { get; init; }
  public IReadOnlyList<CalOutDetOtrosDto>? Otros { get; init; }
}
