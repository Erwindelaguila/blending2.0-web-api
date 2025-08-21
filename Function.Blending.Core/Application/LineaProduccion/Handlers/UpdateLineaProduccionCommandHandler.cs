using MediatR;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
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
