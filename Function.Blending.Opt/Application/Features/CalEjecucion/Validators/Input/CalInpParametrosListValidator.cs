using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Input;

public sealed class CalInpParametrosListValidator : AbstractValidator<IReadOnlyList<CalInpParametroDto>>
{
  public CalInpParametrosListValidator()
  {
    RuleFor(x => x).NotEmpty();

    RuleForEach(x => x)
      .SetValidator(new CalInpParametroDtoValidator());

    RuleFor(x => x)
      .Must(list =>
      {
        var seen = new HashSet<(Guid cal, Guid par)>();
        foreach (var i in list)
        {
          if (!seen.Add((i.CalidadId, i.ParametroId)))
            return false;
        }
        return true;
      })
      .WithMessage("Existen duplicados (CalidadId, ParametroId) en la colección de parámetros.");
  }
}
