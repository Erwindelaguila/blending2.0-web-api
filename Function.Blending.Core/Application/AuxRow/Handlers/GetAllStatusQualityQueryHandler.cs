using Function.Blending.Core.Application.AuxRow.DTOs;
using Function.Blending.Core.Application.AuxRow.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Application.AuxRow.Handlers;

public class GetAllStatusQualityQueryHandler : IRequestHandler<GetAllStatusQualityQuery, List<StatusQualityDTO>>
{
    private readonly IAuxRowRepository _auxRowRepository;
    private readonly ILogger<GetAllStatusQualityQueryHandler> _logger;
    private readonly IConfiguration _configuration;

    public GetAllStatusQualityQueryHandler(IAuxRowRepository auxRowRepository, ILogger<GetAllStatusQualityQueryHandler> logger, IConfiguration configuration)
    {
        _auxRowRepository = auxRowRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<List<StatusQualityDTO>> Handle(GetAllStatusQualityQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo estados de calidad .... ");

        var qualityAuxTableValue = _configuration["Id_Status_Quality"];

        if (string.IsNullOrWhiteSpace(qualityAuxTableValue))
        {
            _logger.LogError("La configuración 'Id_Status_Quality' no está definida.");
            throw new InvalidOperationException("La configuración 'Id_Status_Quality' no está definida.");
        }

        if (!Guid.TryParse(qualityAuxTableValue, out var idQualityAuxTable))
        {
            _logger.LogError("El valor de 'Id_Status_Quality' no es un GUID válido. Valor: {Value}", qualityAuxTableValue);
            throw new InvalidOperationException($"El valor de 'Id_Status_Quality' no es un GUID válido. Valor: {qualityAuxTableValue}");
        }
        

        return await _auxRowRepository.GetStatusQualityAsync(idQualityAuxTable);;
    }
}