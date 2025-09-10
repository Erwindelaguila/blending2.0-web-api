using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Output;

public sealed class CalOutResParametroDtoValidator : AbstractValidator<CalOutResParametroDto>
{
  public CalOutResParametroDtoValidator()
  {
    RuleLevelCascadeMode = CascadeMode.Stop;

    RuleFor(x => x.CodigoParametro)
      .NotEmpty().WithMessage("CodigoParametro es obligatorio.")
      .Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage("CodigoParametro no debe ser blanco.");
  }
}
