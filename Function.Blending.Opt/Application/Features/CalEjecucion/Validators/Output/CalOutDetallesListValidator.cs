using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Output;

public sealed class CalOutDetallesListValidator : AbstractValidator<IReadOnlyList<CalOutDetalleDto>>
{
  public CalOutDetallesListValidator()
  {
    RuleLevelCascadeMode = CascadeMode.Stop;

    RuleForEach(x => x).SetValidator(new CalOutDetalleDtoValidator());

    RuleFor(x => x)
      .Must(NoRumaGrupoDuplicados)
      .WithMessage("Detalles contiene combinaciones duplicadas de (Ruma, Grupo).");

    static bool NoRumaGrupoDuplicados(IReadOnlyList<CalOutDetalleDto>? list)
    {
      if (list is null || list.Count <= 1) return true;

      var keys = list
        .Select(d => (
          ruma: (d.Ruma ?? string.Empty).Trim().ToUpperInvariant(),
          grupo: (d.Grupo ?? string.Empty).Trim().ToUpperInvariant()
        ))
        .Where(k => !string.IsNullOrEmpty(k.ruma) || !string.IsNullOrEmpty(k.grupo))
        .ToList();

      return keys.Distinct().Count() == keys.Count;
    }
  }
}
