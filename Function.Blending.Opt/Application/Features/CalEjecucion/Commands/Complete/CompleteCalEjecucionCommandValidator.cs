using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;
using Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Commands.Complete;

public sealed class CompleteCalEjecucionCommandValidator : AbstractValidator<CompleteCalEjecucionCommand>
{
  public CompleteCalEjecucionCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.EstadoId).NotEmpty();
    RuleFor(x => x.ModificadoPorId).NotEmpty();

    When(x => x.Resumenes is not null, () =>
      RuleFor(x => x.Resumenes!).SetValidator(new CalOutResumenesListValidator()));

    When(x => x.Detalles is not null, () =>
      RuleFor(x => x.Detalles!).SetValidator(new CalOutDetallesListValidator()));
  }
}
