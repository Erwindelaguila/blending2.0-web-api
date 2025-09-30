using FluentValidation;
using Function.Blending.Core.Application.Agregado.Commands;

namespace Function.Blending.Core.Application.Agregado.Validators;

public class UpdateAgregadoValidator : AbstractValidator<UpdateAgregadoCommand>
{
    public UpdateAgregadoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del agregado es requerido");
        
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código del agregado es requerido")
            .MaximumLength(20).WithMessage("El código del agregado no puede exceder 20 caracteres");
        
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del agregado es requerido")
            .MaximumLength(50).WithMessage("El nombre del agregado no puede exceder 50 caracteres");
        
        RuleFor(x => x.Descripcion)
            .MaximumLength(150).WithMessage("La descripción del agregado no puede exceder 150 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descripcion));
    }
}
