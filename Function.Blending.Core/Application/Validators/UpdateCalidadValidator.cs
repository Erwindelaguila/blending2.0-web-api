using FluentValidation;
using Function.Blending.Core.Application.Calidad.Commands;

namespace Function.Blending.Core.Application.Validators;

public class UpdateCalidadValidator : AbstractValidator<UpdateCalidadCommand>
{
    public UpdateCalidadValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador de la calidad es requerido");
        
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