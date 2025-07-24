using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers;

public class GetAllAgregadosQueryHandler : IRequestHandler<GetAllAgregadosQuery, List<AgregadoDTO>>
{
    private readonly IAgregadoRepository _agregadoRepository;

    public GetAllAgregadosQueryHandler(IAgregadoRepository agregadoRepository)
    {
        _agregadoRepository = agregadoRepository;
    }

    public async Task<List<AgregadoDTO>> Handle(GetAllAgregadosQuery request, CancellationToken cancellationToken)
    {
        var agregados = await _agregadoRepository.GetAllAsync();
        return agregados.Select(agregado => new AgregadoDTO
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
