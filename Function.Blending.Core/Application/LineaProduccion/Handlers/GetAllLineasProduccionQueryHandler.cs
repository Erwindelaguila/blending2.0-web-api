using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.LineaProduccion.Queries;
using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class GetAllLineasProduccionQueryHandler : IRequestHandler<GetAllLineasProduccionQuery, object>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;

    public GetAllLineasProduccionQueryHandler(ILineaProduccionRepository lineaProduccionRepository)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
    }

    public async Task<object> Handle(GetAllLineasProduccionQuery request, CancellationToken cancellationToken)
    {
        var (entities, total) = await _lineaProduccionRepository.GetPagedAsync(request.Page, request.Size);
        
        var dtos = entities.Select(linea => new LineaProduccionDTO
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

        return new
        {
            Items = dtos,
            Total = total,
            Page = request.Page,
            Size = request.Size,
            TotalPages = (int)Math.Ceiling((double)total / request.Size)
        };
    }
}
