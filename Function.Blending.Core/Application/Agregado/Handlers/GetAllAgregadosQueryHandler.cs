using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers;

public class GetAllAgregadosQueryHandler : IRequestHandler<GetAllAgregadosQuery, PagedResponse<AgregadoDTO>>
{
    private readonly IAgregadoRepository _agregadoRepository;

    public GetAllAgregadosQueryHandler(IAgregadoRepository agregadoRepository)
    {
        _agregadoRepository = agregadoRepository;
    }

    public async Task<PagedResponse<AgregadoDTO>> Handle(GetAllAgregadosQuery request, CancellationToken cancellationToken)
    {
        var agregadosQuery = _agregadoRepository.GetQueryable()
            .Select(agregado => new AgregadoDTO
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
            });

        var pagedResult = await agregadosQuery.ToPagedResultAsync(request.Page, request.Size, cancellationToken);
        return pagedResult.ToPagedResponse();
    }
}
