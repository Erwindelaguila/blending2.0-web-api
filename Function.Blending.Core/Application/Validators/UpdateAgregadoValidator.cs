using FluentValidation;
using Function.Blending.Core.Application.Agregado.Commands;

namespace Function.Blending.Core.Application.Agregado.Validators;

public class UpdateAgregadoValidator : AbstractValidator<UpdateAgregadoCommand>
{
    public UpdateAgregadoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID es requerido");
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(20).WithMessage("El código no puede tener más de 20 caracteres");
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(50).WithMessage("El nombre no puede tener más de 50 caracteres");
        RuleFor(x => x.Descripcion)
            .MaximumLength(150).WithMessage("La descripción no puede tener más de 150 caracteres");
        When(x => x.ModificadoPorId.HasValue, () =>
        {
            RuleFor(x => x.ModificadoPorId)
                .NotEmpty().WithMessage("El Id del modificador no puede ser vacío");
        });
    }
}
