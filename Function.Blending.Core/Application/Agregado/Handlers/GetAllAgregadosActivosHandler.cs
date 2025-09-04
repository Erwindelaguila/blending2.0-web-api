using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Application.Agregado.Handlers;

public class GetAllAgregadosActivosHandler : IRequestHandler<GetAllAgregadosActivosQuery, List<AgregadoActivoDTO>>
{
    private readonly IAgregadoRepository _repository;
    private readonly ILogger<GetAllAgregadosActivosHandler> _logger;

    public GetAllAgregadosActivosHandler(IAgregadoRepository repository, ILogger<GetAllAgregadosActivosHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<AgregadoActivoDTO>> Handle(GetAllAgregadosActivosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo agregados activos para combo...");
            
            var result = await _repository.GetActivosAsync();
            
            _logger.LogInformation("Se obtuvieron {Count} agregados activos", result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener agregados activos");
            throw;
        }
    }
}
