using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpFilCapacidadDtoValidator : AbstractValidator<LogInpFilCapacidadDto>
  {
    public LogInpFilCapacidadDtoValidator()
    {
      RuleFor(x => x.Cantidad).GreaterThan(0);
      RuleFor(x => x.Capacidad).GreaterThan(0);
    }
  }
}
