using FluentValidation;
using Function.Blending.Core.Application.AppParam.Commands;

namespace Function.Blending.Core.Application.Validators;

public class CreateAppParamValidator : AbstractValidator<CreateAppParamCommand>
{
    public CreateAppParamValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("La clave del parámetro de aplicación es requerida")
            .MaximumLength(100).WithMessage("La clave del parámetro de aplicación no puede exceder 100 caracteres");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("El valor del parámetro de aplicación es requerido")
            .MaximumLength(250).WithMessage("El valor del parámetro de aplicación no puede exceder 250 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(150).WithMessage("La descripción del parámetro de aplicación no puede exceder 150 caracteres");

        RuleFor(x => x.Category)
            .MaximumLength(50).WithMessage("La categoría del parámetro de aplicación no puede exceder 50 caracteres");

        RuleFor(x => x.Group)
            .MaximumLength(50).WithMessage("El grupo del parámetro de aplicación no puede exceder 50 caracteres");

        // CreadoPorId se obtiene automáticamente del JWT - no se valida
    }
}
