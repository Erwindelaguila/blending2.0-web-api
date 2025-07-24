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
            .MaximumLength(20).WithMessage("El código no debe exceder 20 caracteres");
        RuleFor(x => x.Nombre)
            .MaximumLength(50).WithMessage("El nombre no debe exceder 50 caracteres");
        RuleFor(x => x.Descripcion)
            .MaximumLength(150).WithMessage("La descripción no debe exceder 150 caracteres");
        RuleFor(x => x.CalidadId)
            .NotEmpty().When(x => x.CalidadId.HasValue).WithMessage("El Id de calidad es requerido si se especifica");
        RuleFor(x => x.TipoProduccionId)
            .NotEmpty().When(x => x.TipoProduccionId.HasValue).WithMessage("El Id de tipo de producción es requerido si se especifica");
        RuleFor(x => x.ModificadoPorId)
            .NotEmpty().WithMessage("El Id del usuario modificador es requerido");
    }
}
