using FluentValidation;
using Function.Blending.Core.Application.Products.Commands;

namespace Function.Blending.Core.Application.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().WithMessage("El código es obligatorio.");
        RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es obligatorio.");
        RuleFor(x => x.Descripcion).MaximumLength(500);
        RuleFor(x => x.CalidadId).NotEmpty();
        RuleFor(x => x.TipoProduccionId).NotEmpty();
        RuleFor(x => x.CreadoPorId).NotEmpty();
    }
}