using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Extensions;
using Function.Blending.Core.Shared.Constants;
using System.Security.Claims;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;


public class CreateTipoProduccionCommandHandler : IRequestHandler<CreateTipoProduccionCommand, TipoProduccionDTO>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    private readonly IAgregadoRepository _agregadoRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;
    
    public CreateTipoProduccionCommandHandler(
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

    public async Task<TipoProduccionDTO> Handle(CreateTipoProduccionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();

        // Validar que el código no existe
        if (await _tipoProduccionRepository.ExistsActiveCodigoAsync(request.Codigo))
        {
            throw new ArgumentException("El código ya existe", "codigo");
        }

        // Si se intenta crear activo, validar dependencias
        if (request.Activo ?? true)
        {
            await ValidateDependenciesForActivation(request.LineaProduccionId, request.AgregadoId);
        }

        var tipo = new TipoProduccionEntity
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            LineaProduccionId = request.LineaProduccionId,
            AgregadoId = request.AgregadoId,
            Activo = request.Activo ?? true,
            CreadoPorId = currentUserId,
            CreadoEl = DateTime.UtcNow
        };
        
        await _tipoProduccionRepository.CreateAsync(tipo);
        
        var lineaProduccion = await _lineaProduccionRepository.GetByIdAsync(request.LineaProduccionId);
        var agregado = await _agregadoRepository.GetByIdAsync(request.AgregadoId);
        
        return new TipoProduccionDTO
        {
            Id = tipo.Id,
            Codigo = tipo.Codigo,
            Nombre = tipo.Nombre,
            Descripcion = tipo.Descripcion,
            LineaProduccion = new LineaProduccionRelacion 
            { 
                Id = lineaProduccion!.Id, 
                Codigo = lineaProduccion.Codigo 
            },
            Agregado = new AgregadoRelacion 
            { 
                Id = agregado!.Id, 
                Codigo = agregado.Codigo 
            },
            Activo = tipo.Activo,
            CreadoPorId = tipo.CreadoPorId,
            CreadoEl = tipo.CreadoEl,
            ModificadoPorId = tipo.ModificadoPorId,
            ModificadoEl = tipo.ModificadoEl
        };
    }

    private async Task ValidateDependenciesForActivation(Guid lineaProduccionId, Guid agregadoId)
    {
        var lineaProduccion = await _lineaProduccionRepository.GetByIdAsync(lineaProduccionId);
        if (lineaProduccion == null)
            throw new BusinessRuleException($"La línea de producción seleccionada ya no existe o fue eliminada.", 
                "LINEA_PRODUCCION_NOT_FOUND");
        
        if (lineaProduccion.Activo == false)
            throw new BusinessRuleException($"Cannot create/activate TipoProduccion because LineaProduccion '{lineaProduccion.Nombre}' is inactive.", 
                "LINEA_PRODUCCION_INACTIVE");

        // Validar Agregado
        var agregado = await _agregadoRepository.GetByIdAsync(agregadoId);
        if (agregado == null)
            throw new BusinessRuleException($"El agregado seleccionado ya no existe o fue eliminado.", 
                "AGREGADO_NOT_FOUND");
        
        if (agregado.Activo == false)
            throw new BusinessRuleException($"Cannot create/activate TipoProduccion because Agregado '{agregado.Nombre}' is inactive.", 
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
