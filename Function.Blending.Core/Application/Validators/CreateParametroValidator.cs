using FluentValidation;
using Function.Blending.Core.Application.Parametro.Commands;

namespace Function.Blending.Core.Application.Validators;

public class CreateParametroValidator : AbstractValidator<CreateParametroCommand>
{
    public CreateParametroValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código del parámetro es requerido")
            .MaximumLength(20).WithMessage("El código del parámetro no puede exceder 20 caracteres");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del parámetro es requerido")
            .MaximumLength(50).WithMessage("El nombre del parámetro no puede exceder 50 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(150).WithMessage("La descripción del parámetro no puede exceder 150 caracteres");
    }
}
