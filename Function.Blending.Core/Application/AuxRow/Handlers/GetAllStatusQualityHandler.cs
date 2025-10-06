using Function.Blending.Core.Application.AuxRow.DTOs;
using Function.Blending.Core.Application.AuxRow.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.AuxRow.Handlers;

public class GetAllStatusQualityHandler : IRequestHandler<GetAllStatusQualityQuery, List<StatusQualityDTO>>
{
    private readonly IAuxRowRepository _auxRowRepository;

    public GetAllStatusQualityHandler(IAuxRowRepository auxRowRepository)
    {
        _auxRowRepository = auxRowRepository;
    }

    public async Task<List<StatusQualityDTO>> Handle(GetAllStatusQualityQuery request, CancellationToken cancellationToken)
    {
        // Usando el Id_Status_Quality del local.settings.dev
        var statusQualityId = Guid.Parse("98066F2C-9B5D-46E4-A29E-C7D97F8F920C");
        return await _auxRowRepository.GetStatusQualityAsync(statusQualityId);
    }
}