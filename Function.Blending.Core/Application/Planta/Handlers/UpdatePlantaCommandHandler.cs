using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;

using Function.Blending.Core.Application.Planta.Commands;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class UpdatePlantaCommandHandler : IRequestHandler<UpdatePlantaCommand, PlantaDTO>
{
    private readonly IPlantaRepository _plantaRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public UpdatePlantaCommandHandler(IPlantaRepository plantaRepository, IFunctionContextAccessor functionContextAccessor)
    {
        _plantaRepository = plantaRepository;
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<PlantaDTO> Handle(UpdatePlantaCommand request, CancellationToken cancellationToken)
    {
        var modificadoPorId = GetCurrentUserId();

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

    private Guid GetCurrentUserId()
    {
        try
        {
            var context = _functionContextAccessor.Current;
            if (context?.Items.TryGetValue(Function.Blending.Core.Shared.Constants.MiscellaneousConstants.Principal, out var principalObj) == true &&
                principalObj is System.Security.Claims.ClaimsPrincipal principal)
            {
                var userIdString = principal.GetUserId();
                if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
                {
                    return userId;
                }
            }
        }
        catch
        {
            // Si hay error obteniendo el usuario, usar fallback
        }
        
        // Fallback: usuario del sistema
        return Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}
