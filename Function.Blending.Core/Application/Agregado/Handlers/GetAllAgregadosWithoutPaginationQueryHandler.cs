using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers;

public class GetAllAgregadosWithoutPaginationQueryHandler : IRequestHandler<GetAllAgregadosWithoutPaginationQuery, List<AgregadoDTO>>
{
    private readonly IAgregadoRepository _agregadoRepository;

    public GetAllAgregadosWithoutPaginationQueryHandler(IAgregadoRepository agregadoRepository)
    {
        _agregadoRepository = agregadoRepository;
    }

    public async Task<List<AgregadoDTO>> Handle(GetAllAgregadosWithoutPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _agregadoRepository.GetQueryable();
        query = query.OrderBy(x => x.CreadoEl);
        var entities = await Task.FromResult(query.ToList());

        return entities.Select(agregado => new AgregadoDTO
        {
            Id = agregado.Id,
            Codigo = agregado.Codigo,
            Nombre = agregado.Nombre,
            Descripcion = agregado.Descripcion,
            Activo = agregado.Activo,
            CreadoPorId = agregado.CreadoPorId,
            CreadoEl = agregado.CreadoEl,
            ModificadoPorId = agregado.ModificadoPorId,
            ModificadoEl = agregado.ModificadoEl
        }).ToList();
    }
}
