using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Planta.Queries;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class GetPlantaByIdQueryHandler : IRequestHandler<GetPlantaByIdQuery, PlantaDTO?>
{
    private readonly IPlantaRepository _plantaRepository;

    public GetPlantaByIdQueryHandler(IPlantaRepository plantaRepository)
    {
        _plantaRepository = plantaRepository;
    }

    public async Task<PlantaDTO?> Handle(GetPlantaByIdQuery request, CancellationToken cancellationToken)
    {
        var planta = await _plantaRepository.GetByIdAsync(request.Id);
        
        if (planta == null)
            return null;
            
        return new PlantaDTO
        {
            Id = planta.Id,
            Codigo = planta.Codigo,
            Nombre = planta.Nombre,
            Descripcion = planta.Descripcion,
            NumeroRuma = planta.NumeroRuma,
            Activo = planta.Activo,
            CreadoPorId = planta.CreadoPorId,
            CreadoEl = planta.CreadoEl,
            ModificadoPorId = planta.ModificadoPorId,
            ModificadoEl = planta.ModificadoEl
        };
    }
}
