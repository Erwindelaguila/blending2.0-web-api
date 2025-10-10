using MediatR;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using Function.Blending.Core.Shared.Security;
using System.Security.Claims;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class UpdateLineaProduccionCommandHandler : IRequestHandler<UpdateLineaProduccionCommand, LineaProduccionDTO>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public UpdateLineaProduccionCommandHandler(ILineaProduccionRepository lineaProduccionRepository, IFunctionContextAccessor functionContextAccessor)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<LineaProduccionDTO> Handle(UpdateLineaProduccionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();

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

    private Guid GetCurrentUserId()
    {
        try
        {
            var context = _functionContextAccessor.Current;
            if (context?.Items.TryGetValue(MiscellaneousConstants.Principal, out var principalObj) == true &&
                principalObj is ClaimsPrincipal principal)
            {
                var userIdString = principal.GetUserId();
                if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
                {
                    return userId;
                }
            }
        }
        catch
        {
            // Si hay error obteniendo el usuario, usar fallback
        }
        
        // Fallback: usuario del sistema
        return Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}
