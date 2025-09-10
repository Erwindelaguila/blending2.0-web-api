using FluentValidation;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Commands.Complete
{
  public sealed class CompleteLogEjecucionCommandValidator : AbstractValidator<CompleteLogEjecucionCommand>
  {
    public CompleteLogEjecucionCommandValidator()
    {
      RuleFor(x => x.Id).NotEmpty();
      RuleFor(x => x.EstadoId).NotEmpty();        // Igual que Calidad: estado es requerido
      RuleFor(x => x.ModificadoPorId).NotEmpty(); // auditoría
      // Mensaje opcional (como Calidad)
    }
  }
}
