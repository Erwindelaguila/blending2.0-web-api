using FluentValidation;
using Function.Blending.Core.Application.CalidadParametro.Commands;

namespace Function.Blending.Core.Application.CalidadParametro.Validators;

public class UpsertCalidadParametroCommandValidator : AbstractValidator<UpsertCalidadParametroCommand>
{
    public UpsertCalidadParametroCommandValidator()
    {
        RuleFor(x => x.CalidadId)
            .NotEmpty()
            .WithMessage("El ID de calidad es requerido.");

        RuleFor(x => x.ParametroId)
            .NotEmpty()
            .WithMessage("El ID de parámetro es requerido.");

        RuleFor(x => x.Valor)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El valor debe ser mayor o igual a 0.");
    }
}
