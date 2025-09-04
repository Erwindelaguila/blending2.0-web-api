using MediatR;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class UpdateLineaProduccionCommandHandler : IRequestHandler<UpdateLineaProduccionCommand, LineaProduccionDTO>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    private readonly IAuthorizationService _authorizationService;
    
    public UpdateLineaProduccionCommandHandler(
        ILineaProduccionRepository lineaProduccionRepository,
        IAuthorizationService authorizationService)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
        _authorizationService = authorizationService;
    }

    public async Task<LineaProduccionDTO> Handle(UpdateLineaProduccionCommand request, CancellationToken cancellationToken)
    {
        var currentUserIdString = _authorizationService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            throw new InvalidOperationException("User ID inválido en headers");
        }

        var currentLineaProduccion = await _lineaProduccionRepository.GetByIdAsync(request.Id);
        if (currentLineaProduccion == null)
            throw new InvalidOperationException("Línea de Producción no encontrada");

        if (currentLineaProduccion.Activo && request.Activo == false)
        {
            var isUsedByActiveTipoProduccion = await _lineaProduccionRepository.IsUsedByActiveTipoProduccionAsync(request.Id);
            if (isUsedByActiveTipoProduccion)
            {
                throw new EntityInUseException("la Línea de Producción", "está siendo usada por al menos un Tipo de Producción activo");
            }
        }

        var lineaProduccionEntity = new LineaProduccionEntity
        {
            Id = request.Id,
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = request.Activo ?? true,
            ModificadoPorId = currentUserId,
            ModificadoEl = DateTime.UtcNow
        };

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
