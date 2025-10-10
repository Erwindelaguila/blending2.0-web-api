using FluentValidation;
using Function.Blending.Core.Application.Planta.Commands;

namespace Function.Blending.Core.Application.Validators;

public class CreatePlantaValidator : AbstractValidator<CreatePlantaCommand>
{
    public CreatePlantaValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código de la planta es requerido.")
            .MaximumLength(20).WithMessage("El código de la planta no puede exceder 20 caracteres.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la planta es requerido.")
            .MaximumLength(50).WithMessage("El nombre de la planta no puede exceder 50 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(150).WithMessage("La descripción no puede exceder 150 caracteres.");

        RuleFor(x => x.NumeroRuma)
            .GreaterThan(0).WithMessage("El número de ruma debe ser mayor a 0.");
    }
}
