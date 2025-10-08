using Function.Blending.Core.Application.AuxRow.DTOs;
using Function.Blending.Core.Application.AuxRow.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Core.Application.AuxRow.Handlers;

public class GetAllStatusQualityHandler : IRequestHandler<GetAllStatusQualityQuery, List<StatusRowDTO>>
{
    private readonly IAuxRowRepository _auxRowRepository;
    private readonly IConfiguration _configuration;

    public GetAllStatusQualityHandler(IAuxRowRepository auxRowRepository, IConfiguration configuration)
    {
        _auxRowRepository = auxRowRepository;
        _configuration = configuration;
    }

    public async Task<List<StatusRowDTO>> Handle(GetAllStatusQualityQuery request, CancellationToken cancellationToken)
    {
        // Usando el Id_Status_Quality del local.settings.dev
        var statusQualityId = Guid.Parse(_configuration["Id_Status_Quality"] ??
                                         throw new ArgumentNullException("Id_Status_Quality no está configurado."));
        return await _auxRowRepository.GetStatusQualityAsync(statusQualityId);
    }
}