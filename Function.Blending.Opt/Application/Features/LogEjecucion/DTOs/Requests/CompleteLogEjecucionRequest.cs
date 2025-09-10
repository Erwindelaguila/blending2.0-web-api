using System;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests;

public sealed class CompleteLogEjecucionRequest
{
  public Guid Id { get; init; }
  public Guid EstadoId { get; init; }
  public string? Mensaje { get; init; }
}
