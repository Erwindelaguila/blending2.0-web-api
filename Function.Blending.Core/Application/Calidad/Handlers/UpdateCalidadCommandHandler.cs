using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class UpdateCalidadCommandHandler : IRequestHandler<UpdateCalidadCommand, object>
{
    private readonly ICalidadRepository _repository;

    public UpdateCalidadCommandHandler(ICalidadRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> Handle(UpdateCalidadCommand request, CancellationToken cancellationToken)
    {
        var calidad = new CalidadEntity
        {
            Id = request.Id,
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            CodigoMaterial = request.CodigoMaterial,
            Descripcion = request.Descripcion,
            NoConforme = request.NoConforme ?? false,
            Activo = request.Activo ?? true,
            ModificadoPorId = request.ModificadoPorId,
            ModificadoEl = DateTime.UtcNow
        };

        var calidadActualizada = await _repository.UpdateAndReturnAsync(calidad);

        return new CalidadDTO
        {
            Id = calidadActualizada.Id,
            Codigo = calidadActualizada.Codigo,
            Nombre = calidadActualizada.Nombre,
            CodigoMaterial = calidadActualizada.CodigoMaterial,
            Descripcion = calidadActualizada.Descripcion,
            NoConforme = calidadActualizada.NoConforme,
            Activo = calidadActualizada.Activo,
            CreadoPorId = calidadActualizada.CreadoPorId,
            CreadoEl = calidadActualizada.CreadoEl,
            ModificadoPorId = calidadActualizada.ModificadoPorId,
            ModificadoEl = calidadActualizada.ModificadoEl,
        };
    }
}