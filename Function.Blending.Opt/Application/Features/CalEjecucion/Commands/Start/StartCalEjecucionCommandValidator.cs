using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;
using Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Input;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.Start;

public sealed class StartCalEjecucionCommandValidator : AbstractValidator<StartCalEjecucionCommand>
{
  public StartCalEjecucionCommandValidator()
  {
    RuleFor(x => x.PlantaId).NotEmpty();
    RuleFor(x => x.CreadoPorId).NotEmpty();

    When(x => x.Filtro is not null, () =>
    {
      RuleFor(x => x.Filtro!).SetValidator(new CalInpFiltroDtoValidator());
    });

    When(x => x.Parametros is not null, () =>
    {
      RuleFor(x => x.Parametros!).SetValidator(new CalInpParametrosListValidator());
    });
  }
}
