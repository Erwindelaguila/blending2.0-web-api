using FluentValidation;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Validators.Input
{
  public sealed class LogInpOfertaListValidator : AbstractValidator<IReadOnlyList<LogInpOfertaDto>?>
  {
    public LogInpOfertaListValidator()
    {
      When(x => x is not null, () =>
      {
        RuleFor(x => x!).NotEmpty();

        RuleForEach(x => x!)
          .SetValidator(new LogInpOfertaDtoValidator());
      });
    }
  }
}
