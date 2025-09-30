using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input;

public sealed class LogisticaModelPayloadValidator : AbstractValidator<LogisticaModelPayload>
{
  public LogisticaModelPayloadValidator()
  {
    RuleFor(x => x.Demanda).NotNull();
    RuleFor(x => x.Demanda.Cantidad).GreaterThan(0);

    RuleFor(x => x.Oferta).NotNull()
        .Must(x => x.Count > 0).WithMessage("Se requiere al menos un lote de oferta.");

    RuleForEach(x => x.Oferta).ChildRules(of =>
    {
      of.RuleFor(o => o.Lote).NotEmpty();
      of.RuleFor(o => o.CantidadAsignada).GreaterThanOrEqualTo(0);
      of.RuleFor(o => o.Parametros).NotNull();
    });

    RuleFor(x => x.Contenedores).NotNull();
    RuleFor(x => x.ParametrosSeleccionados).NotNull();
    RuleFor(x => x.IndiceOferta).NotNull();
    RuleFor(x => x.OfertaSacos).NotNull();

    RuleFor(x => x.Capacidades).NotNull();
    RuleForEach(x => x.Capacidades).ChildRules(c =>
    {
      c.RuleFor(p => p.Cantidad).GreaterThan(0);
      c.RuleFor(p => p.Capacidad).GreaterThan(0);
    });

    RuleFor(x => x.Particiones).NotNull();
    RuleFor(x => x.PesoContenedor).GreaterThan(0);
    RuleFor(x => x.NroMovimientos).GreaterThanOrEqualTo(0);
    RuleFor(x => x.Emparejamientos).NotNull();
    RuleFor(x => x.TiempoEspera).GreaterThanOrEqualTo(0);
  }
}
