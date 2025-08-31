using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Planta.Commands;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class UpdatePlantaCommandHandler : IRequestHandler<UpdatePlantaCommand, PlantaDTO>
{
    private readonly IPlantaRepository _plantaRepository;
    private readonly IAuthorizationService _authorizationService;

    public UpdatePlantaCommandHandler(IPlantaRepository plantaRepository, IAuthorizationService authorizationService)
    {
        _plantaRepository = plantaRepository;
        _authorizationService = authorizationService;
    }

    public async Task<PlantaDTO> Handle(UpdatePlantaCommand request, CancellationToken cancellationToken)
    {
        var modificadoPorIdString = _authorizationService.GetCurrentUserId();
        var modificadoPorId = Guid.Parse(modificadoPorIdString);

        var plantaToUpdate = new PlantaEntity
        {
            Id = request.Id,
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            NumeroRuma = request.NumeroRuma,
            Activo = request.Activo ?? true,
            ModificadoPorId = modificadoPorId,
            ModificadoEl = DateTime.UtcNow
        };

        var updatedPlanta = await _plantaRepository.UpdateAndReturnAsync(plantaToUpdate);
        
        return new PlantaDTO
        {
            Id = updatedPlanta.Id,
            Codigo = updatedPlanta.Codigo,
            Nombre = updatedPlanta.Nombre,
            Descripcion = updatedPlanta.Descripcion,
            NumeroRuma = updatedPlanta.NumeroRuma,
            Activo = updatedPlanta.Activo,
            CreadoPorId = updatedPlanta.CreadoPorId,
            CreadoEl = updatedPlanta.CreadoEl,
            ModificadoPorId = updatedPlanta.ModificadoPorId,
            ModificadoEl = updatedPlanta.ModificadoEl
        };
    }
}
