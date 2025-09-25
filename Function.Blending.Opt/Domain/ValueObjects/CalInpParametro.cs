using Function.Blending.Opt.Domain.ValueObjects.Ids;

namespace Function.Blending.Opt.Domain.ValueObjects;

public sealed record CalInpParametro(
  Guid? Id,
  Guid? EjecucionId,
  CalidadId CalidadId,
  ParametroId ParametroId,
  decimal Valor
);
