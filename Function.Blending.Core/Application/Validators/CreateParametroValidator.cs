using FluentValidation;
using Function.Blending.Core.Application.Parametro.Commands;

namespace Function.Blending.Core.Application.Validators;

public class CreateParametroValidator : AbstractValidator<CreateParametroCommand>
{
    public CreateParametroValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(20).WithMessage("El código no puede tener más de 20 caracteres");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(50).WithMessage("El nombre no puede tener más de 50 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(150).WithMessage("La descripción no puede tener más de 150 caracteres");

        RuleFor(x => x.CreadoPorId)
            .NotEmpty().WithMessage("El ID del usuario creador es requerido");
    }
}
