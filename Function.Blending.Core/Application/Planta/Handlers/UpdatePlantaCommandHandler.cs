using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Planta.Commands;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class UpdatePlantaCommandHandler : IRequestHandler<UpdatePlantaCommand, PlantaDTO>
{
    private readonly IPlantaRepository _plantaRepository;

    public UpdatePlantaCommandHandler(IPlantaRepository plantaRepository)
    {
        _plantaRepository = plantaRepository;
    }

    public async Task<PlantaDTO> Handle(UpdatePlantaCommand request, CancellationToken cancellationToken)
    {
        var planta = await _plantaRepository.GetByIdAsync(request.Id);
        
        if (planta == null)
            throw new ArgumentException($"Planta con ID {request.Id} no encontrada");

        planta.Codigo = request.Codigo ?? planta.Codigo;
        planta.Nombre = request.Nombre ?? planta.Nombre;
        planta.Descripcion = request.Descripcion ?? planta.Descripcion;
        planta.NumeroRuma = request.NumeroRuma;
        planta.Activo = request.Activo ?? planta.Activo;
        planta.ModificadoPorId = request.ModificadoPorId;
        planta.ModificadoEl = DateTime.Now;

        await _plantaRepository.UpdateAsync(planta);
        
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
