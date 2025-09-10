using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Output;

public sealed class CalOutResumenDtoValidator : AbstractValidator<CalOutResumenDto>
{
  public CalOutResumenDtoValidator()
  {
    RuleLevelCascadeMode = CascadeMode.Stop;

    RuleForEach(x => x.Parametros)
      .SetValidator(new CalOutResParametroDtoValidator());

    RuleFor(x => x.Parametros)
      .Must(NoCodigosDuplicados)
      .WithMessage("Parametros contiene códigos duplicados.");

    static bool NoCodigosDuplicados(IReadOnlyList<CalOutResParametroDto>? list)
    {
      if (list is null || list.Count <= 1) return true;
      var keys = list.Select(p => (p.CodigoParametro ?? string.Empty).Trim().ToUpperInvariant());
      return keys.Distinct().Count() == list.Count;
    }
  }
}
