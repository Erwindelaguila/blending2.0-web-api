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
        var (entities, total) = await _tipoProduccionRepository.GetPagedAsync(request.Page, request.Size);
        
        var dtos = entities.Select(tipo => new TipoProduccionDTO
        {
            Id = tipo.Id,
            Codigo = tipo.Codigo,
            Nombre = tipo.Nombre,
            Descripcion = tipo.Descripcion,
            LineaProduccionId = tipo.LineaProduccionId,
            AgregadoId = tipo.AgregadoId,
            Activo = tipo.Activo,
            CreadoPorId = tipo.CreadoPorId,
            CreadoEl = tipo.CreadoEl,
            ModificadoPorId = tipo.ModificadoPorId,
            ModificadoEl = tipo.ModificadoEl
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
