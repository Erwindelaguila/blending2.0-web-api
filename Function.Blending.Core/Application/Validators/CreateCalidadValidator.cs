using FluentValidation;
using Function.Blending.Core.Application.Calidad.Commands;

namespace Function.Blending.Core.Application.Validators;

public class CreateCalidadValidator :AbstractValidator<CreateCalidadCommand>
{
    public CreateCalidadValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre de calidad es requerido");
        RuleFor(x=>x.Codigo).NotEmpty().WithMessage("El código es requerida");
        RuleFor(x=>x.CodigoMaterial).NotEmpty().WithMessage("El código de material es requerida");
        RuleFor(x => x.Descripcion).MaximumLength(500);
        RuleFor(x => x.CreadoPorId).NotEmpty().WithMessage("El Id del creado es requerido");
    }
}