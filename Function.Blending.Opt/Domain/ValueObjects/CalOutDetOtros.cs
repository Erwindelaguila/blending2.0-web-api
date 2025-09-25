namespace Function.Blending.Opt.Domain.ValueObjects;

public sealed record CalOutDetOtros(
  Guid? Id = default,
  Guid? DetalleId = default,
  string? Codigo = null, 
  string? Valor = null
);
