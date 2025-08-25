using MediatR;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class GetTipoProduccionByIdQueryHandler : IRequestHandler<GetTipoProduccionByIdQuery, TipoProduccionDTO?>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    public GetTipoProduccionByIdQueryHandler(ITipoProduccionRepository tipoProduccionRepository)
    {
        _tipoProduccionRepository = tipoProduccionRepository;
    }

    public async Task<TipoProduccionDTO?> Handle(GetTipoProduccionByIdQuery request, CancellationToken cancellationToken)
    {
        // Usar el método que devuelve estructura anidada
        return await _tipoProduccionRepository.GetByIdWithRelationsAsync(request.Id);
    }
}
