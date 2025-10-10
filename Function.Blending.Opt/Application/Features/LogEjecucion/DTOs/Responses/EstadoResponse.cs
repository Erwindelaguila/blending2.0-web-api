using System;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses
{
  public sealed class EstadoResponse
  {
    public Guid Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Color { get; init; }
  }
}
