using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpDemParametrosListValidator : AbstractValidator<IReadOnlyList<LogInpDemParametroDto>?>
  {
    public LogInpDemParametrosListValidator()
    {
      // La lista es opcional: si viene, valida elementos y duplicados por CodigoParametro.
      When(x => x is not null, () =>
      {
        RuleFor(x => x!).NotEmpty();

        RuleForEach(x => x!)
          .SetValidator(new LogInpDemParametroDtoValidator());

        RuleFor(x => x!)
          .Must(list =>
          {
            var seen = new HashSet<string>();
            foreach (var i in list)
            {
              if (!seen.Add(i.CodigoParametro))
                return false;
            }
            return true;
          })
          .WithMessage("Existen duplicados de CodigoParametro en la colección de parámetros de demanda.");
      });
    }
  }
}
