using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpOfeParametroDtoValidator : AbstractValidator<LogInpOfeParametroDto>
  {
    public LogInpOfeParametroDtoValidator()
    {
      RuleFor(x => x.CodigoParametro).NotEmpty().MaximumLength(20);
      // Valor: decimal, no se impone signo aquí.
    }
  }
}
