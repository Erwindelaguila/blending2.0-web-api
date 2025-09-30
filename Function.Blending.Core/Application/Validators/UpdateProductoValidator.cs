using FluentValidation;
using Function.Blending.Core.Application.Producto.Commands;

namespace Function.Blending.Core.Application.Validators;

public class UpdateProductoValidator : AbstractValidator<UpdateProductoCommand>
{
    public UpdateProductoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El identificador del producto es requerido");
        
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código del producto es requerido")
            .MaximumLength(20).WithMessage("El código del producto no puede exceder 20 caracteres");
        
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del producto es requerido")
            .MaximumLength(50).WithMessage("El nombre del producto no puede exceder 50 caracteres");
        
        RuleFor(x => x.Descripcion)
            .MaximumLength(150).WithMessage("La descripción del producto no puede exceder 150 caracteres");
        
        RuleFor(x => x.CalidadId)
            .NotEmpty().WithMessage("La calidad del producto es requerida");
        
        RuleFor(x => x.TipoProduccionId)
            .NotEmpty().WithMessage("El tipo de producción del producto es requerido");
    }
}
