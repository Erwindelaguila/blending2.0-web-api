using FluentValidation;
using Function.Blending.Core.Application.LineaProduccion.Commands;

namespace Function.Blending.Core.Application.Validators;

public class UpdateLineaProduccionValidator : AbstractValidator<UpdateLineaProduccionCommand>
{
    public UpdateLineaProduccionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID de la línea de producción es requerido");
        
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código de la línea de producción es requerido")
            .MaximumLength(20).WithMessage("El código de la línea de producción no puede exceder 20 caracteres");
        
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la línea de producción es requerido")
            .MaximumLength(50).WithMessage("El nombre de la línea de producción no puede exceder 50 caracteres");
        
        RuleFor(x => x.Descripcion)
            .MaximumLength(150).WithMessage("La descripción de la línea de producción no puede exceder 150 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descripcion));
    }
}
