using FluentValidation;
using Function.Blending.Core.Application.Producto.Commands;

namespace Function.Blending.Core.Application.Validators;

public class UpdateProductoValidator : AbstractValidator<UpdateProductoCommand>
{
    public UpdateProductoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id del producto es requerido");
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(20).WithMessage("El código no debe exceder 20 caracteres");
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(50).WithMessage("El nombre no debe exceder 50 caracteres");
        RuleFor(x => x.Descripcion)
            .MaximumLength(150).WithMessage("La descripción no debe exceder 150 caracteres");
        RuleFor(x => x.CalidadId)
            .NotEmpty().WithMessage("El Id de calidad es requerido");
        RuleFor(x => x.TipoProduccionId)
            .NotEmpty().WithMessage("El Id de tipo de producción es requerido");
    }
}
