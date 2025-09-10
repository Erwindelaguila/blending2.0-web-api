using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpFiltroDtoValidator : AbstractValidator<LogInpFiltroDto>
  {
    public LogInpFiltroDtoValidator()
    {
      RuleFor(x => x.PesoContenedor).GreaterThan(0);

      // Colecciones en PLURAL + null-guards
      When(x => x.Capacidades is not null, () =>
      {
        RuleForEach(x => x.Capacidades!).SetValidator(new LogInpFilCapacidadDtoValidator());
      });

      When(x => x.Divisiones is not null, () =>
      {
        RuleForEach(x => x.Divisiones!).SetValidator(new LogInpFilDivisionDtoValidator());
      });

      When(x => x.Emparejamientos is not null, () =>
      {
        RuleForEach(x => x.Emparejamientos!).SetValidator(new LogInpFilEmparejamientoDtoValidator());
      });
    }
  }
}
