using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.LineaProduccion.Queries;
using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class GetLineaProduccionByIdQueryHandler : IRequestHandler<GetLineaProduccionByIdQuery, LineaProduccionDTO?>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;

    public GetLineaProduccionByIdQueryHandler(ILineaProduccionRepository lineaProduccionRepository)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
    }

    public async Task<LineaProduccionDTO?> Handle(GetLineaProduccionByIdQuery request, CancellationToken cancellationToken)
    {
        var linea = await _lineaProduccionRepository.GetByIdAsync(request.Id);

        if (linea == null)
            return null;
        return new LineaProduccionDTO
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
        };
    }
}
