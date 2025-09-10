using FluentValidation;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.ToggleState;

public sealed class ToggleCalEstadoCommandValidator : AbstractValidator<ToggleCalEstadoCommand>
{
  public ToggleCalEstadoCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.ModificadoPorId).NotEmpty();
  }
}
