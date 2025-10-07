using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpOfertaDtoValidator : AbstractValidator<LogInpOfertaDto>
  {
    public LogInpOfertaDtoValidator()
    {
      RuleFor(x => x.Posicion).GreaterThanOrEqualTo(0);
      RuleFor(x => x.Ruma).NotEmpty().MaximumLength(20);

      RuleFor(x => x.DescripcionMaterial).NotEmpty().MaximumLength(100);
      RuleFor(x => x.DescripcionMaterial).NotEmpty().MaximumLength(100);

      RuleFor(x => x.UnidadMedidaVenta).MaximumLength(10);
      RuleFor(x => x.UnidadMedidaAlmacen).MaximumLength(10);

      RuleFor(x => x.CantidadAsignadaVenta).GreaterThanOrEqualTo(0);
      RuleFor(x => x.CantidadAsignadaAlmacen).GreaterThanOrEqualTo(0);

      RuleFor(x => x.FechaContabilizacion).NotEmpty().MaximumLength(10);
      RuleFor(x => x.FechaFabricacion).NotEmpty().MaximumLength(10);

      RuleFor(x => x.FechaAnalisisQuimico).MaximumLength(10);
      RuleFor(x => x.FechaVencimientoQuimico).MaximumLength(10);
      RuleFor(x => x.FechaAnalisisMicrobiologico).MaximumLength(10);
      RuleFor(x => x.FechaVencimientoMicrobiologico).MaximumLength(10);

      RuleFor(x => x.Parametros).SetValidator(new LogInpOfeParametrosListValidator());
      RuleFor(x => x.Otros).SetValidator(new LogInpOfeOtrosListValidator());
    }
  }
}
