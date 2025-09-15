using FluentValidation;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.ChangeAccepted;

public sealed class ChangeAcceptedCalEjecucionCommandValidator : AbstractValidator<ChangeAcceptedCalEjecucionCommand>
{
  public ChangeAcceptedCalEjecucionCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.ModificadoPorId).NotEmpty();

    When(x => x.Grupos is not null, () => RuleFor(x => x.Grupos!).NotEmpty());
  }
}