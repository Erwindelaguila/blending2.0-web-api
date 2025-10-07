using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpDemParametroDtoValidator : AbstractValidator<LogInpDemParametroDto>
  {
    public LogInpDemParametroDtoValidator()
    {
      RuleFor(x => x.CodigoParametro).NotEmpty().MaximumLength(20);
    }
  }
}
