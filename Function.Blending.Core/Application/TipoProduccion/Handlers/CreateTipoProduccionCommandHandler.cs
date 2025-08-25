using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class CreateTipoProduccionCommandHandler : IRequestHandler<CreateTipoProduccionCommand, TipoProduccionDTO>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    private readonly IAgregadoRepository _agregadoRepository;
    
    public CreateTipoProduccionCommandHandler(
        ITipoProduccionRepository tipoProduccionRepository,
        ILineaProduccionRepository lineaProduccionRepository,
        IAgregadoRepository agregadoRepository)
    {
        _tipoProduccionRepository = tipoProduccionRepository;
        _lineaProduccionRepository = lineaProduccionRepository;
        _agregadoRepository = agregadoRepository;
    }

    public async Task<TipoProduccionDTO> Handle(CreateTipoProduccionCommand request, CancellationToken cancellationToken)
    {
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
            CreadoPorId = request.CreadoPorId,
            CreadoEl = DateTime.UtcNow
        };
        await _tipoProduccionRepository.CreateAsync(tipo);
        
        // Devolver la estructura anidada
        return await _tipoProduccionRepository.GetByIdWithRelationsAsync(tipo.Id);
    }

    private async Task ValidateDependenciesForActivation(Guid lineaProduccionId, Guid agregadoId)
    {
        // Validar LineaProduccion
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
