using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class GetTipoProduccionByIdWithRelationsQueryHandler : IRequestHandler<GetTipoProduccionByIdWithRelationsQuery, TipoProduccionDTO?>
{
    private readonly ITipoProduccionRepository _repository;
    private readonly ILogger<GetTipoProduccionByIdWithRelationsQueryHandler> _logger;

    public GetTipoProduccionByIdWithRelationsQueryHandler(
        ITipoProduccionRepository repository,
        ILogger<GetTipoProduccionByIdWithRelationsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TipoProduccionDTO?> Handle(GetTipoProduccionByIdWithRelationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo tipo de producción con relaciones. ID: {Id}", request.Id);
            
            var result = await _repository.GetByIdWithRelationsAsync(request.Id);
            
            if (result == null)
            {
                _logger.LogWarning("Tipo de producción no encontrado. ID: {Id}", request.Id);
                return null;
            }
            
            _logger.LogInformation("Tipo de producción obtenido exitosamente. ID: {Id}, Código: {Codigo}", 
                result.Id, result.Codigo);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipo de producción con relaciones. ID: {Id}", request.Id);
            throw;
        }
    }
}
