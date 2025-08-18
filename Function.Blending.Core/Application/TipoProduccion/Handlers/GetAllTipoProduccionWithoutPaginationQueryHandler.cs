using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class GetAllTipoProduccionWithoutPaginationQueryHandler : IRequestHandler<GetAllTipoProduccionWithoutPaginationQuery, List<TipoProduccionDTO>>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;

    public GetAllTipoProduccionWithoutPaginationQueryHandler(ITipoProduccionRepository tipoProduccionRepository)
    {
        _tipoProduccionRepository = tipoProduccionRepository;
    }

    public async Task<List<TipoProduccionDTO>> Handle(GetAllTipoProduccionWithoutPaginationQuery request, CancellationToken cancellationToken)
    {
        var entities = await _tipoProduccionRepository.GetAllAsync();
        
        return entities.Select(tipoProduccion => new TipoProduccionDTO
        {
            Id = tipoProduccion.Id,
            Codigo = tipoProduccion.Codigo,
            Nombre = tipoProduccion.Nombre,
            Descripcion = tipoProduccion.Descripcion,
            LineaProduccionId = tipoProduccion.LineaProduccionId,
            AgregadoId = tipoProduccion.AgregadoId,
            Activo = tipoProduccion.Activo,
            CreadoPorId = tipoProduccion.CreadoPorId,
            CreadoEl = tipoProduccion.CreadoEl,
            ModificadoPorId = tipoProduccion.ModificadoPorId,
            ModificadoEl = tipoProduccion.ModificadoEl
        }).ToList();
    }
}
