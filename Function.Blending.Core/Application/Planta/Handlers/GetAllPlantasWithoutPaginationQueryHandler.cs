using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Planta.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class GetAllPlantasWithoutPaginationQueryHandler : IRequestHandler<GetAllPlantasWithoutPaginationQuery, List<PlantaDTO>>
{
    private readonly IPlantaRepository _plantaRepository;

    public GetAllPlantasWithoutPaginationQueryHandler(IPlantaRepository plantaRepository)
    {
        _plantaRepository = plantaRepository;
    }

    public async Task<List<PlantaDTO>> Handle(GetAllPlantasWithoutPaginationQuery request, CancellationToken cancellationToken)
    {
        var entities = await _plantaRepository.GetAllAsync();
        
        return entities.Select(planta => new PlantaDTO
        {
            Id = planta.Id,
            Codigo = planta.Codigo,
            Nombre = planta.Nombre,
            Descripcion = planta.Descripcion,
            Activo = planta.Activo,
            CreadoPorId = planta.CreadoPorId,
            CreadoEl = planta.CreadoEl,
            ModificadoPorId = planta.ModificadoPorId,
            ModificadoEl = planta.ModificadoEl
        }).ToList();
    }
}
