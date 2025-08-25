using MediatR;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class UpdateLineaProduccionCommandHandler : IRequestHandler<UpdateLineaProduccionCommand, LineaProduccionDTO>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    
    public UpdateLineaProduccionCommandHandler(ILineaProduccionRepository lineaProduccionRepository)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
    }

    public async Task<LineaProduccionDTO> Handle(UpdateLineaProduccionCommand request, CancellationToken cancellationToken)
    {
        // Obtener el registro actual para verificar cambios
        var currentLineaProduccion = await _lineaProduccionRepository.GetByIdAsync(request.Id);
        if (currentLineaProduccion == null)
            throw new InvalidOperationException("Línea de Producción no encontrada");

        // Validar regla de negocio: no se puede inactivar si está siendo usado por TipoProducción activo
        if (currentLineaProduccion.Activo && request.Activo == false)
        {
            var isUsedByActiveTipoProduccion = await _lineaProduccionRepository.IsUsedByActiveTipoProduccionAsync(request.Id);
            if (isUsedByActiveTipoProduccion)
            {
                throw new EntityInUseException("la Línea de Producción", "está siendo usada por al menos un Tipo de Producción activo");
            }
        }

        // Crear una nueva entidad limpia con los datos del comando
        var lineaProduccionEntity = new LineaProduccionEntity
        {
            Id = request.Id,
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = request.Activo ?? true,
            ModificadoPorId = request.ModificadoPorId,
            ModificadoEl = DateTime.UtcNow
        };

        // Usar el método UpdateAndReturnAsync que maneja la validación y preserva los campos necesarios
        var updatedEntity = await _lineaProduccionRepository.UpdateAndReturnAsync(lineaProduccionEntity);

        return new LineaProduccionDTO
        {
            Id = updatedEntity.Id,
            Codigo = updatedEntity.Codigo,
            Nombre = updatedEntity.Nombre,
            Descripcion = updatedEntity.Descripcion,
            Activo = updatedEntity.Activo,
            CreadoPorId = updatedEntity.CreadoPorId,
            CreadoEl = updatedEntity.CreadoEl,
            ModificadoPorId = updatedEntity.ModificadoPorId,
            ModificadoEl = updatedEntity.ModificadoEl
        };
    }
}
