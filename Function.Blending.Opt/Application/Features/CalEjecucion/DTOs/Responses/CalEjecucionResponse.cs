using System;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;

public sealed class CalEjecucionResponse
{
  public Guid Id { get; init; }
  public Guid PlantaId { get; init; }
  public string? Codigo { get; init; }
  public DateTime CreadoEl { get; init; } // UTC

  // Anidado (nuevo)
  public EstadoResponse? Estado { get; init; }
}
