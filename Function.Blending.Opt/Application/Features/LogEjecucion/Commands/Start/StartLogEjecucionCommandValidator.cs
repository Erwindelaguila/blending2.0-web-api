using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.Start
{
  public sealed class StartLogEjecucionCommandValidator : AbstractValidator<StartLogEjecucionCommand>
  {
    public StartLogEjecucionCommandValidator()
    {
      RuleFor(x => x.CreadoPorId).NotEmpty();
      RuleFor(x => x.Mensaje).MaximumLength(250);

      When(x => x.Info is not null, () =>
      {
        RuleFor(x => x.Info!).SetValidator(new LogInpInfoDtoValidator());
      });

      When(x => x.Filtro is not null, () =>
      {
        RuleFor(x => x.Filtro!).SetValidator(new LogInpFiltroDtoValidator());
      });

      When(x => x.Oferta is not null, () =>
      {
        RuleFor(x => x.Oferta!).SetValidator(new LogInpOfertaDtoValidator());
      });
    }
  }
}
