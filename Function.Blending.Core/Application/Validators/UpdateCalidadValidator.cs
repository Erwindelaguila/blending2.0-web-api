using FluentValidation;
using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Calidad.Handlers;

namespace Function.Blending.Core.Application.Validators;

public class UpdateCalidadValidator: AbstractValidator<UpdateCalidadCommand>
{
    public UpdateCalidadValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El campo id es requerido");
        RuleFor(x => x.Descripcion).MaximumLength(500);
        RuleFor(x=>x.ModificadoPorId).NotEmpty().WithMessage("El campo modificadorPorId es requerido");
    }
    
}