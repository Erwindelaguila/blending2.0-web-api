using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.Start
{
  public sealed class StartLogEjecucionCommandValidator : AbstractValidator<StartLogEjecucionCommand>
  {
    public StartLogEjecucionCommandValidator()
    {
      RuleFor(x => x.CreadoPorId).NotEmpty();
      RuleFor(x => x.Start.Mensaje).MaximumLength(250);

      When(x => x.Start.Info is not null, () =>
      {
        RuleFor(x => x.Start.Info!).SetValidator(new LogInpInfoDtoValidator());
      });

      When(x => x.Start.Filtro is not null, () =>
      {
        RuleFor(x => x.Start.Filtro!).SetValidator(new LogInpFiltroDtoValidator());
      });

      When(x => x.Start.Oferta is not null, () =>
      {
        RuleFor(x => x.Start.Oferta!).SetValidator(new LogInpOfertaDtoValidator());
      });

      When(x => x.Model is not null, () =>
      {
        RuleFor(x => x.Model!).SetValidator(new LogisticaModelPayloadValidator());
      });
    }
  }
}
