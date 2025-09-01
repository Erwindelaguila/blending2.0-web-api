using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;


public class CreateTipoProduccionCommandHandler : IRequestHandler<CreateTipoProduccionCommand, TipoProduccionDTO>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    private readonly IAgregadoRepository _agregadoRepository;
    private readonly IAuthorizationService _authorizationService;
    
    public CreateTipoProduccionCommandHandler(
        ITipoProduccionRepository tipoProduccionRepository,
        ILineaProduccionRepository lineaProduccionRepository,
        IAgregadoRepository agregadoRepository,
        IAuthorizationService authorizationService)
    {
        _tipoProduccionRepository = tipoProduccionRepository ?? throw new ArgumentNullException(nameof(tipoProduccionRepository));
        _lineaProduccionRepository = lineaProduccionRepository ?? throw new ArgumentNullException(nameof(lineaProduccionRepository));
        _agregadoRepository = agregadoRepository ?? throw new ArgumentNullException(nameof(agregadoRepository));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
    }

    public async Task<TipoProduccionDTO> Handle(CreateTipoProduccionCommand request, CancellationToken cancellationToken)
    {
        // Obtener usuario actual para auditoría
        var currentUserIdString = _authorizationService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            throw new UnauthorizedAccessException("User ID inválido en headers");
        }

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
}
