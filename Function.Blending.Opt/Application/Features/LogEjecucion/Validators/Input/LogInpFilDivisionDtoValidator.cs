using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpFilDivisionDtoValidator : AbstractValidator<LogInpFilDivisionDto>
  {
    public LogInpFilDivisionDtoValidator()
    {
      RuleFor(x => x.Ruma).NotEmpty();
      RuleFor(x => x.Division).NotEmpty();
    }
  }
}
