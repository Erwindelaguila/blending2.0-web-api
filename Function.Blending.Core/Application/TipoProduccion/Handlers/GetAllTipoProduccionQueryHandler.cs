using MediatR;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class GetAllTipoProduccionQueryHandler : IRequestHandler<GetAllTipoProduccionQuery, object>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    public GetAllTipoProduccionQueryHandler(ITipoProduccionRepository tipoProduccionRepository)
    {
        _tipoProduccionRepository = tipoProduccionRepository;
    }

    public async Task<object> Handle(GetAllTipoProduccionQuery request, CancellationToken cancellationToken)
    {
        // Usar el método con relaciones para estructura consistente
        var dtos = await _tipoProduccionRepository.GetAllWithRelationsAsync();
        
        // Aplicar paginación en memoria (temporal, se puede optimizar moviendo a repositorio)
        var pagedDtos = dtos
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .ToList();

        return new
        {
            Items = pagedDtos,
            Total = dtos.Count,
            Page = request.Page,
            Size = request.Size,
            TotalPages = (int)Math.Ceiling((double)dtos.Count / request.Size)
        };
    }
}
