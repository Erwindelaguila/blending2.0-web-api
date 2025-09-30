using FluentValidation;
using Function.Blending.Core.Application.TipoProduccion.Commands;

namespace Function.Blending.Core.Application.Validators;

public class UpdateTipoProduccionValidator : AbstractValidator<UpdateTipoProduccionCommand>
{
    public UpdateTipoProduccionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del tipo de producción es requerido");
        
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código del tipo de producción es requerido")
            .MaximumLength(20).WithMessage("El código del tipo de producción no puede exceder 20 caracteres");
        
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del tipo de producción es requerido")
            .MaximumLength(50).WithMessage("El nombre del tipo de producción no puede exceder 50 caracteres");
        
        RuleFor(x => x.Descripcion)
            .MaximumLength(150).WithMessage("La descripción del tipo de producción no puede exceder 150 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descripcion));
        
        RuleFor(x => x.LineaProduccionId)
            .NotEmpty().WithMessage("La línea de producción es requerida");
        
        RuleFor(x => x.AgregadoId)
            .NotEmpty().WithMessage("El agregado es requerido");
    }
}
