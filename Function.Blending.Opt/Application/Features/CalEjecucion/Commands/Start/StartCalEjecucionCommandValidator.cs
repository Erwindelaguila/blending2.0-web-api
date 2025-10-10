using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;
using Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Input;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.Start;

public sealed class StartCalEjecucionCommandValidator : AbstractValidator<StartCalEjecucionCommand>
{
  public StartCalEjecucionCommandValidator()
  {
    RuleFor(x => x.Start).NotEmpty();
    RuleFor(x => x.Model).NotEmpty();
    RuleFor(x => x.CreadoPorId).NotEmpty();

    RuleFor(x => x.Start.PlantaId).NotEmpty();

    When(x => x.Start.Filtro is not null, () =>
    {
      RuleFor(x => x.Start.Filtro!).SetValidator(new CalInpFiltroDtoValidator());
    });

    When(x => x.Start.Parametros is not null, () =>
    {
      RuleFor(x => x.Start.Parametros!).SetValidator(new CalInpParametrosListValidator());
    });
  }
}
