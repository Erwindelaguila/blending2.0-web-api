using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.LineaProduccion.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class GetAllLineasProduccionActivasHandler : IRequestHandler<GetAllLineasProduccionActivasQuery, List<LineaProduccionActivaDTO>>
{
    private readonly ILineaProduccionRepository _repository;
    private readonly ILogger<GetAllLineasProduccionActivasHandler> _logger;

    public GetAllLineasProduccionActivasHandler(ILineaProduccionRepository repository, ILogger<GetAllLineasProduccionActivasHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<LineaProduccionActivaDTO>> Handle(GetAllLineasProduccionActivasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo líneas de producción activas para combo...");
            
            var result = await _repository.GetActivasAsync();
            
            _logger.LogInformation("Se obtuvieron {Count} líneas de producción activas", result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener líneas de producción activas");
            throw;
        }
    }
}
