using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class GetAllTipoProduccionActivasHandler : IRequestHandler<GetAllTipoProduccionActivasQuery, List<TipoProduccionActivaDTO>>
{
    private readonly ITipoProduccionRepository _repository;
    private readonly ILogger<GetAllTipoProduccionActivasHandler> _logger;

    public GetAllTipoProduccionActivasHandler(ITipoProduccionRepository repository, ILogger<GetAllTipoProduccionActivasHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<TipoProduccionActivaDTO>> Handle(GetAllTipoProduccionActivasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo tipos de producción activos para combo...");
            
            var result = await _repository.GetActivasAsync();
            
            _logger.LogInformation("Se obtuvieron {Count} tipos de producción activos", result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de producción activos");
            throw;
        }
    }
}
