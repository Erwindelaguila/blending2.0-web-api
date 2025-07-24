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
        var tipo = await _tipoProduccionRepository.GetByIdAsync(request.Id);
        if (tipo == null)
            return null;
        return new TipoProduccionDTO
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
        };
    }
}
