using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Output;

public sealed class CalOutDetalleDtoValidator : AbstractValidator<CalOutDetalleDto>
{
  public CalOutDetalleDtoValidator()
  {
    RuleLevelCascadeMode = CascadeMode.Stop;

    RuleForEach(x => x.Parametros)
      .SetValidator(new CalOutDetParametroDtoValidator());

    RuleFor(x => x.Parametros)
      .Must(NoCodigosDuplicadosParametro)
      .WithMessage("Parametros contiene códigos duplicados.");

    RuleForEach(x => x.Otros)
      .SetValidator(new CalOutDetOtrosDtoValidator());

    RuleFor(x => x.Otros)
      .Must(NoCodigosDuplicadosOtros)
      .WithMessage("Otros contiene códigos duplicados.");

    static bool NoCodigosDuplicadosParametro(IReadOnlyList<CalOutDetParametroDto>? list)
    {
      if (list is null || list.Count <= 1) return true;
      var keys = list.Select(p => (p.CodigoParametro ?? string.Empty).Trim().ToUpperInvariant());
      return keys.Distinct().Count() == list.Count;
    }

    static bool NoCodigosDuplicadosOtros(IReadOnlyList<CalOutDetOtrosDto>? list)
    {
      if (list is null || list.Count <= 1) return true;
      var keys = list.Select(p => (p.Codigo ?? string.Empty).Trim().ToUpperInvariant());
      return keys.Distinct().Count() == list.Count;
    }
  }
}
