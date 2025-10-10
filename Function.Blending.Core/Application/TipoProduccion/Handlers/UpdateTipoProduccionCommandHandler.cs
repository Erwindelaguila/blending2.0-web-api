using MediatR;
using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Extensions;
using Function.Blending.Core.Shared.Constants;
using System.Security.Claims;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;


public class UpdateTipoProduccionCommandHandler : IRequestHandler<UpdateTipoProduccionCommand, TipoProduccionDTO>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    private readonly IAgregadoRepository _agregadoRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;
    
    public UpdateTipoProduccionCommandHandler(
        ITipoProduccionRepository tipoProduccionRepository,
        ILineaProduccionRepository lineaProduccionRepository,
        IAgregadoRepository agregadoRepository,
        IFunctionContextAccessor functionContextAccessor)
    {
        _tipoProduccionRepository = tipoProduccionRepository ?? throw new ArgumentNullException(nameof(tipoProduccionRepository));
        _lineaProduccionRepository = lineaProduccionRepository ?? throw new ArgumentNullException(nameof(lineaProduccionRepository));
        _agregadoRepository = agregadoRepository ?? throw new ArgumentNullException(nameof(agregadoRepository));
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<TipoProduccionDTO> Handle(UpdateTipoProduccionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();

        var currentTipo = await _tipoProduccionRepository.GetByIdAsync(request.Id);
        if (currentTipo == null)
            throw new BusinessRuleException($"TipoProduccion with ID {request.Id} not found.", 
                "TIPO_PRODUCCION_NOT_FOUND");

        if (request.Activo == true && currentTipo.Activo == false)
        {
            await ValidateDependenciesForActivation(request.LineaProduccionId, request.AgregadoId);
        }

        if (currentTipo.Activo == true && 
            (request.LineaProduccionId != currentTipo.LineaProduccionId || 
             request.AgregadoId != currentTipo.AgregadoId))
        {
            await ValidateDependenciesForActivation(request.LineaProduccionId, request.AgregadoId);
        }

        if (currentTipo.Activo && request.Activo == false)
        {
            var isUsedByActiveProducto = await _tipoProduccionRepository.IsUsedByActiveProductoAsync(request.Id);
            if (isUsedByActiveProducto)
            {
                throw new EntityInUseException("el Tipo de Producción", "está siendo usado por al menos un Producto activo");
            }
        }

        var tipoToUpdate = new TipoProduccionEntity
        {
            Id = request.Id,
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            LineaProduccionId = request.LineaProduccionId,
            AgregadoId = request.AgregadoId,
            Activo = request.Activo ?? true,
            ModificadoPorId = currentUserId, 
            ModificadoEl = DateTime.UtcNow
        };

        var updatedTipo = await _tipoProduccionRepository.UpdateAndReturnAsync(tipoToUpdate);


        var result = await _tipoProduccionRepository.GetByIdWithRelationsAsync(updatedTipo.Id);
        if (result == null)
        {
            throw new BusinessRuleException($"Error al recuperar el TipoProduccion actualizado con ID {updatedTipo.Id}", 
                "TIPO_PRODUCCION_RETRIEVAL_ERROR");
        }
        
        return result;
    }

    private async Task ValidateDependenciesForActivation(Guid lineaProduccionId, Guid agregadoId)
    {
        var lineaProduccion = await _lineaProduccionRepository.GetByIdAsync(lineaProduccionId);
        if (lineaProduccion == null)
            throw new BusinessRuleException($"La línea de producción seleccionada ya no existe o fue eliminada.", 
                "LINEA_PRODUCCION_NOT_FOUND");
        
        if (lineaProduccion.Activo == false)
            throw new BusinessRuleException($"No se puede activar: línea de producción '{lineaProduccion.Nombre}' inactiva.", 
                "LINEA_PRODUCCION_INACTIVE");

        // Validar Agregado
        var agregado = await _agregadoRepository.GetByIdAsync(agregadoId);
        if (agregado == null)
            throw new BusinessRuleException($"El agregado seleccionado ya no existe o fue eliminado.", 
                "AGREGADO_NOT_FOUND");
        
        if (agregado.Activo == false)
            throw new BusinessRuleException($"No se puede activar: agregado '{agregado.Nombre}' inactivo.", 
                "AGREGADO_INACTIVE");
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
