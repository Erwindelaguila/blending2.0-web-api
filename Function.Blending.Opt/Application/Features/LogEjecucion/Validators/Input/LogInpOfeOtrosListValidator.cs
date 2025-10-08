using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpOfeOtrosListValidator : AbstractValidator<IReadOnlyList<LogInpOfeOtrosDto>?>
  {
    public LogInpOfeOtrosListValidator()
    {
      // La lista es opcional: si viene, valida elementos y duplicados por Codigo.
      When(x => x is not null, () =>
      {
        RuleFor(x => x!).NotEmpty();

        RuleForEach(x => x!)
          .SetValidator(new LogInpOfeOtrosDtoValidator());

        RuleFor(x => x!)
          .Must(list =>
          {
            var seen = new HashSet<string>();
            foreach (var i in list)
            {
              if (!seen.Add(i.Codigo))
                return false;
            }
            return true;
          })
          .WithMessage("Existen duplicados de Codigo en la colección de otros de oferta.");
      });
    }
  }
}
