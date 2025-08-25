using FluentValidation;
using Function.Blending.Core.Application.AppParam.Commands;

namespace Function.Blending.Core.Application.Validators;

public class UpdateAppParamValidator : AbstractValidator<UpdateAppParamCommand>
{
    public UpdateAppParamValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("La clave es requerida")
            .MaximumLength(100).WithMessage("La clave no puede exceder 100 caracteres");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("El valor es requerido")
            .MaximumLength(250).WithMessage("El valor no puede exceder 250 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(150).WithMessage("La descripción no puede exceder 150 caracteres");

        RuleFor(x => x.Category)
            .MaximumLength(50).WithMessage("La categoría no puede exceder 50 caracteres");

        RuleFor(x => x.Group)
            .MaximumLength(50).WithMessage("El grupo no puede exceder 50 caracteres");

        RuleFor(x => x.ModificadoPorId)
            .NotEmpty().WithMessage("El ID del usuario modificador es requerido");
    }
}
