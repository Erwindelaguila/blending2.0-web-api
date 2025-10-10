using FluentValidation;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.ToggleState;

public sealed class ToggleLogConfirmadoCommandValidator : AbstractValidator<ToggleLogConfirmadoCommand>
{
  public ToggleLogConfirmadoCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.ModificadoPorId).NotEmpty();
  }
}