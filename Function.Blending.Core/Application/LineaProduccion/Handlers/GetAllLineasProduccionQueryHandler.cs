using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.LineaProduccion.Queries;
using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class GetAllLineasProduccionQueryHandler : IRequestHandler<GetAllLineasProduccionQuery, List<LineaProduccionDTO>>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;

    public GetAllLineasProduccionQueryHandler(ILineaProduccionRepository lineaProduccionRepository)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
    }

    public async Task<List<LineaProduccionDTO>> Handle(GetAllLineasProduccionQuery request, CancellationToken cancellationToken)
    {
        var lineas = await _lineaProduccionRepository.GetAllAsync();
        return lineas.Select(linea => new LineaProduccionDTO
        {
            Id = linea.Id,
            Codigo = linea.Codigo,
            Nombre = linea.Nombre,
            Descripcion = linea.Descripcion,
            Activo = linea.Activo,
            CreadoPorId = linea.CreadoPorId,
            CreadoEl = linea.CreadoEl,
            ModificadoPorId = linea.ModificadoPorId,
            ModificadoEl = linea.ModificadoEl
        }).ToList();
    }
}
