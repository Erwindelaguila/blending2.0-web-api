using FluentValidation;
using Function.Blending.Core.Application.Agregado.Commands;

namespace Function.Blending.Core.Application.Agregado.Validators;

public class CreateAgregadoValidator : AbstractValidator<CreateAgregadoCommand>
{
    public CreateAgregadoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(20).WithMessage("El código no debe exceder 20 caracteres");
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(50).WithMessage("El nombre no debe exceder 50 caracteres");
        RuleFor(x => x.Descripcion)
            .MaximumLength(150).WithMessage("La descripción no debe exceder 150 caracteres");
        // CreadoPorId ya no se valida aquí porque se extrae del JWT
    }
}
