using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpOfertaDtoValidator : AbstractValidator<LogInpOfertaDto>
  {
    public LogInpOfertaDtoValidator()
    {
      RuleFor(x => x.Posicion).GreaterThanOrEqualTo(0);

      RuleFor(x => x.Material).NotEmpty().MaximumLength(20);
      RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(50);

      RuleFor(x => x.CantidadAsignadaVenta).GreaterThanOrEqualTo(0);
      RuleFor(x => x.UnidadMedidaVenta).GreaterThan(0);

      RuleFor(x => x.CantidadAsignadaAlmacen).GreaterThanOrEqualTo(0);
      RuleFor(x => x.UnidadMedidaAlmacen).GreaterThan(0);

      RuleFor(x => x.Parametros).SetValidator(new LogInpOfeParametrosListValidator());
    }
  }
}
