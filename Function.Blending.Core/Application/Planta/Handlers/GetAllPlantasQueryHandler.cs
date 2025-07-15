using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Planta.Queries;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class GetAllPlantasQueryHandler : IRequestHandler<GetAllPlantasQuery, List<PlantaDTO>>
{
    private readonly IPlantaRepository _plantaRepository;

    public GetAllPlantasQueryHandler(IPlantaRepository plantaRepository)
    {
        _plantaRepository = plantaRepository;
    }

    public async Task<List<PlantaDTO>> Handle(GetAllPlantasQuery request, CancellationToken cancellationToken)
    {
        var plantas = await _plantaRepository.GetAllAsync();
        
        return plantas.Select(p => new PlantaDTO
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            NumeroRuma = p.NumeroRuma,
            Activo = p.Activo,
            CreadoPorId = p.CreadoPorId,
            CreadoEl = p.CreadoEl,
            ModificadoPorId = p.ModificadoPorId,
            ModificadoEl = p.ModificadoEl
        }).ToList();
    }
}
