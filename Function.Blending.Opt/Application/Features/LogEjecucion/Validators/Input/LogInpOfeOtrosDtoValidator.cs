using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpOfeOtrosDtoValidator : AbstractValidator<LogInpOfeOtrosDto>
  {
    public LogInpOfeOtrosDtoValidator()
    {
      RuleFor(x => x.Codigo).NotEmpty().MaximumLength(20);
    }
  }
}
