using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Common.Exceptions;
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
        // Obtener el registro actual para validar cambios
        var currentCalidad = await _repository.GetByIdAsync(request.Id);
        if (currentCalidad == null)
            throw new BusinessRuleException($"Calidad with ID {request.Id} not found.", 
                "CALIDAD_NOT_FOUND");

        // Si se intenta inactivar, validar que no esté siendo usado por Producto activo
        if (currentCalidad.Activo && request.Activo == false)
        {
            var isUsedByActiveProducto = await _repository.IsUsedByActiveProductoAsync(request.Id);
            if (isUsedByActiveProducto)
            {
                throw new EntityInUseException("la Calidad", "está siendo usada por al menos un Producto activo");
            }
        }

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