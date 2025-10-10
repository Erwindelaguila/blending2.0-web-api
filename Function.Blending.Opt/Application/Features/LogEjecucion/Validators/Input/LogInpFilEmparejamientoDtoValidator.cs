using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpFilEmparejamientoDtoValidator : AbstractValidator<LogInpFilEmparejamientoDto>
  {
    public LogInpFilEmparejamientoDtoValidator()
    {
      RuleFor(x => x.Grupo).NotEmpty();
      RuleFor(x => x.CodigoParametro).NotEmpty();
    }
  }
}
