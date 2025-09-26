using System;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;

public sealed class CalEjecucionHistoryItemResponse
{
  public Guid Id { get; init; }
  public string? Codigo { get; init; }
  public Guid EstadoId { get; init; }
  public string? EstadoNombre { get; init; }
  public string? EstadoColor { get; init; }
  public Guid PlantaId { get; init; }
  public DateTime CreadoEl { get; init; }
}
