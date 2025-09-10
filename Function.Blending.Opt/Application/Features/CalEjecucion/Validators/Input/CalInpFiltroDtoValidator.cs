using FluentValidation;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Validators.Input;

public sealed class CalInpFiltroDtoValidator : AbstractValidator<CalInpFiltroDto>
{
  public CalInpFiltroDtoValidator()
  {
    RuleFor(x => x.CentroUbicacion).NotEmpty().MaximumLength(200);
    RuleFor(x => x.CentroProduccion).NotEmpty().MaximumLength(200);
    RuleFor(x => x.UbicacionAlmacen).NotEmpty().MaximumLength(200);
    RuleFor(x => x.TipoProduccion).NotEmpty().MaximumLength(200);

    RuleFor(x => x.BorrarCalidades)
      .MaximumLength(200)
      .When(x => x.BorrarCalidades is not null);

    RuleFor(x => x.ValorCadmioAlto).GreaterThan(0m);

    RuleFor(x => x.NumeroRuma)
      .GreaterThanOrEqualTo(0)
      .When(x => x.NumeroRuma.HasValue);

    RuleFor(x => x.DivisionRuma)
      .GreaterThanOrEqualTo(0)
      .When(x => x.DivisionRuma.HasValue);
  }
}
