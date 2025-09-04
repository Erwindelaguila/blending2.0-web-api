using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Planta.Commands;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class CreatePlantaCommandHandler : IRequestHandler<CreatePlantaCommand, PlantaDTO>
{
    private readonly IPlantaRepository _plantaRepository;
    private readonly IAuthorizationService _authorizationService;

    public CreatePlantaCommandHandler(IPlantaRepository plantaRepository, IAuthorizationService authorizationService)
    {
        _plantaRepository = plantaRepository;
        _authorizationService = authorizationService;
    }

    public async Task<PlantaDTO> Handle(CreatePlantaCommand request, CancellationToken cancellationToken)
    {
        var creadoPorIdString = _authorizationService.GetCurrentUserId();
        var creadoPorId = Guid.Parse(creadoPorIdString);

        var planta = new PlantaEntity
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            NumeroRuma = request.NumeroRuma,
            Activo = request.Activo ?? true,
            CreadoPorId = creadoPorId,
            CreadoEl = DateTime.UtcNow,
            Eliminado = false
        };

        await _plantaRepository.CreateAsync(planta);

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
