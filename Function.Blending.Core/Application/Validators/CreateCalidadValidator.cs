using FluentValidation;
using Function.Blending.Core.Application.Calidad.Commands;

namespace Function.Blending.Core.Application.Validators;

public class CreateCalidadValidator : AbstractValidator<CreateCalidadCommand>
{
    public CreateCalidadValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre de calidad es requerido");
        RuleFor(x => x.Codigo).NotEmpty().WithMessage("El código es requerido");
        RuleFor(x => x.CodigoMaterial).NotEmpty().WithMessage("El código de material es requerido");
        RuleFor(x => x.Descripcion).MaximumLength(500).WithMessage("La descripción no puede tener más de 500 caracteres");
    }
}