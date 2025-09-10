using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Output;

public sealed class CalOutDetOtrosDtoValidator : AbstractValidator<CalOutDetOtrosDto>
{
  public CalOutDetOtrosDtoValidator()
  {
    RuleLevelCascadeMode = CascadeMode.Stop;

    RuleFor(x => x.Codigo)
      .NotEmpty().WithMessage("Codigo es obligatorio.")
      .Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage("Codigo no debe ser blanco.");

    RuleFor(x => x.Valor)
      .NotEmpty().WithMessage("Valor es obligatorio.")
      .Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage("Valor no debe ser blanco.");
  }
}
