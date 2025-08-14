using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Planta.Commands;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class UpdatePlantaCommandHandler : IRequestHandler<UpdatePlantaCommand, object>
{
    private readonly IPlantaRepository _plantaRepository;

    public UpdatePlantaCommandHandler(IPlantaRepository plantaRepository)
    {
        _plantaRepository = plantaRepository;
    }

    public async Task<object> Handle(UpdatePlantaCommand request, CancellationToken cancellationToken)
    {
        var planta = new PlantaEntity
        {
            Id = request.Id,
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            NumeroRuma = request.NumeroRuma,
            Activo = request.Activo ?? true,
            ModificadoPorId = request.ModificadoPorId,
            ModificadoEl = DateTime.UtcNow
        };

        var plantaActualizada = await _plantaRepository.UpdateAndReturnAsync(planta);
        
        return new PlantaDTO
        {
            Id = plantaActualizada.Id,
            Codigo = plantaActualizada.Codigo,
            Nombre = plantaActualizada.Nombre,
            Descripcion = plantaActualizada.Descripcion,
            NumeroRuma = plantaActualizada.NumeroRuma,
            Activo = plantaActualizada.Activo,
            CreadoPorId = plantaActualizada.CreadoPorId,
            CreadoEl = plantaActualizada.CreadoEl,
            ModificadoPorId = plantaActualizada.ModificadoPorId,
            ModificadoEl = plantaActualizada.ModificadoEl
        };
    }
}
