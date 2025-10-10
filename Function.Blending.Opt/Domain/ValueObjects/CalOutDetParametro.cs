namespace Function.Blending.Opt.Domain.ValueObjects;

public sealed record CalOutDetParametro(
  Guid? Id = default,
  Guid? DetalleId = default,
  string? CodigoParametro = null, 
  decimal? Valor = null
);
