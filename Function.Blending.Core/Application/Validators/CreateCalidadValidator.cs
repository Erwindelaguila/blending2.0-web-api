using FluentValidation;
using Function.Blending.Core.Application.Calidad.Commands;

namespace Function.Blending.Core.Application.Validators;

public class CreateCalidadValidator : AbstractValidator<CreateCalidadCommand>
{
    public CreateCalidadValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código de la calidad es requerido")
            .MaximumLength(20).WithMessage("El código de la calidad no puede exceder 20 caracteres");
        
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la calidad es requerido")
            .MaximumLength(100).WithMessage("El nombre de la calidad no puede exceder 100 caracteres");
        
        RuleFor(x => x.CodigoMaterial)
            .NotEmpty().WithMessage("El código de material es requerido")
            .MaximumLength(50).WithMessage("El código de material no puede exceder 50 caracteres");
        
        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción de la calidad no puede exceder 500 caracteres");
    }
}