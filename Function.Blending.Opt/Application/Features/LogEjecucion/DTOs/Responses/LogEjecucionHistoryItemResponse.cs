using System;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;

public sealed class LogEjecucionHistoryItemResponse
{
  public Guid Id { get; init; }
  public string? Codigo { get; init; }
  public string? Contrato { get; init; }
  public bool? Confirmado { get; init; }
  public Guid EstadoId { get; init; }
  public string? EstadoNombre { get; init; }
  public DateTime CreadoEl { get; init; }
}
