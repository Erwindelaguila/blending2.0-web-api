using System;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;

public sealed class StartCalEjecucionResponse
{
  public Guid Id { get; init; }
  public Guid PlantaId { get; init; }
  public string Codigo { get; init; } = string.Empty;
  public DateTime CreadoEl { get; init; } // UTC

  public EstadoResponse? Estado { get; init; }
}
