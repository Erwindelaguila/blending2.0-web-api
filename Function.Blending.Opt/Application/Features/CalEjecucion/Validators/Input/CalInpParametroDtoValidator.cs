using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Input;

public sealed class CalInpParametroDtoValidator : AbstractValidator<CalInpParametroDto>
{
  public CalInpParametroDtoValidator()
  {
    RuleFor(x => x.CalidadId).NotEmpty();
    RuleFor(x => x.ParametroId).NotEmpty();
    RuleFor(x => x.Valor).GreaterThanOrEqualTo(0m);
  }
}
