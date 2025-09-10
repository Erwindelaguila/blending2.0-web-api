using System;

namespace Function.Blending.Opt.Domain.ValueObjects;

public sealed record EstadoLogisticaRef(Guid Id)
{
  public string Nombre { get; init; } = "";
  public string? Color { get; init; }
}
