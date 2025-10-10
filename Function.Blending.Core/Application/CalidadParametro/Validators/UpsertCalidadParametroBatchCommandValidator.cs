using FluentValidation;
using Function.Blending.Core.Application.CalidadParametro.Commands;

namespace Function.Blending.Core.Application.CalidadParametro.Validators;

public class UpsertCalidadParametroBatchCommandValidator : AbstractValidator<UpsertCalidadParametroBatchCommand>
{
    public UpsertCalidadParametroBatchCommandValidator()
    {
        RuleFor(x => x.Cambios)
            .NotEmpty()
            .WithMessage("Debe especificar al menos un cambio.");

        RuleForEach(x => x.Cambios)
            .SetValidator(new CalidadParametroCambioValidator());
    }
}

public class CalidadParametroCambioValidator : AbstractValidator<CalidadParametroCambio>
{
    public CalidadParametroCambioValidator()
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
