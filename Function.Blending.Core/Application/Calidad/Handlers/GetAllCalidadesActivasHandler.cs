using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Calidad.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class GetAllCalidadesActivasHandler : IRequestHandler<GetAllCalidadesActivasQuery, List<CalidadActivaDTO>>
{
    private readonly ICalidadRepository _repository;
    private readonly ILogger<GetAllCalidadesActivasHandler> _logger;

    public GetAllCalidadesActivasHandler(ICalidadRepository repository, ILogger<GetAllCalidadesActivasHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<CalidadActivaDTO>> Handle(GetAllCalidadesActivasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo calidades activas para combo...");
            
            var result = await _repository.GetActivasAsync();
            
            _logger.LogInformation("Se obtuvieron {Count} calidades activas", result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener calidades activas");
            throw;
        }
    }
}
