using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpInfoDtoValidator : AbstractValidator<LogInpInfoDto>
  {
    public LogInpInfoDtoValidator()
    {
      RuleFor(x => x.Contrato).NotEmpty();
      RuleFor(x => x.PedidoVenta).NotEmpty();
      RuleFor(x => x.FechaCarguio).NotEmpty();

      RuleFor(x => x.PlantaCodigo).NotEmpty();
      RuleFor(x => x.PlantaDescripcion).NotEmpty();
      RuleFor(x => x.AlmacenCodigo).NotEmpty();
      RuleFor(x => x.AlmacenDescripcion).NotEmpty();

      RuleFor(x => x.Cliente).NotEmpty();
      RuleFor(x => x.Asistente).NotEmpty();
      RuleFor(x => x.Supervisora).NotEmpty();
      RuleFor(x => x.PaisDestino).NotEmpty();

      RuleFor(x => x.CantidadRuma).GreaterThanOrEqualTo(0);
      RuleFor(x => x.UnidadMedidaRuma).NotEmpty();
      RuleFor(x => x.NumeroMovimientos).GreaterThanOrEqualTo(0);
    }
  }
}
