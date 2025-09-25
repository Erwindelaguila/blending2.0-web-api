namespace Function.Blending.Opt.Domain.ValueObjects;

public sealed record CalOutResParametro(
  Guid? Id = default,
  Guid? ResumenId = default,
  string? CodigoParametro = null, 
  decimal? Valor = null
);
