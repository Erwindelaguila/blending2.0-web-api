using System;

namespace Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;

public sealed record EstadoLogisticaSnapshot(Guid Id)
{
  public string Nombre { get; init; } = "";
  public string? Color { get; init; }
}
