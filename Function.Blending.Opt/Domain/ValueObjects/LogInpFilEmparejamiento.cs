using System;
using Function.Blending.Opt.Domain.ValueObjects.Ids;

namespace Function.Blending.Opt.Domain.ValueObjects
{
  public sealed record LogInpFilEmparejamiento(
    string Grupo,
    ParametroId ParametroId,
    decimal Valor
  );
}
