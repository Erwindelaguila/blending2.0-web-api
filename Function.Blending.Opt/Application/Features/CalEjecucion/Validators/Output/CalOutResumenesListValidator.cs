using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Output;

public sealed class CalOutResumenesListValidator : AbstractValidator<IReadOnlyList<CalOutResumenDto>>
{
  public CalOutResumenesListValidator()
  {
    RuleLevelCascadeMode = CascadeMode.Stop;

    RuleForEach(x => x).SetValidator(new CalOutResumenDtoValidator());

    RuleFor(x => x)
      .Must(NoGruposDuplicados)
      .WithMessage("Resumenes contiene grupos duplicados.");

    static bool NoGruposDuplicados(IReadOnlyList<CalOutResumenDto>? list)
    {
      if (list is null || list.Count <= 1) return true;
      var keys = list
        .Select(r => (r.Grupo ?? string.Empty).Trim().ToUpperInvariant())
        .Where(g => !string.IsNullOrEmpty(g));
      return keys.Distinct().Count() == keys.Count();
    }
  }
}
