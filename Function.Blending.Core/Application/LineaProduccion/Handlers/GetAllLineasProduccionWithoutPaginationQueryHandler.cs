using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.LineaProduccion.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class GetAllLineasProduccionWithoutPaginationQueryHandler : IRequestHandler<GetAllLineasProduccionWithoutPaginationQuery, List<LineaProduccionDTO>>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;

    public GetAllLineasProduccionWithoutPaginationQueryHandler(ILineaProduccionRepository lineaProduccionRepository)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
    }

    public async Task<List<LineaProduccionDTO>> Handle(GetAllLineasProduccionWithoutPaginationQuery request, CancellationToken cancellationToken)
    {
        var entities = await _lineaProduccionRepository.GetAllAsync();
        
        return entities.Select(lineaProduccion => new LineaProduccionDTO
        {
            Id = lineaProduccion.Id,
            Codigo = lineaProduccion.Codigo,
            Nombre = lineaProduccion.Nombre,
            Descripcion = lineaProduccion.Descripcion,
            Activo = lineaProduccion.Activo,
            CreadoPorId = lineaProduccion.CreadoPorId,
            CreadoEl = lineaProduccion.CreadoEl,
            ModificadoPorId = lineaProduccion.ModificadoPorId,
            ModificadoEl = lineaProduccion.ModificadoEl
        }).ToList();
    }
}
