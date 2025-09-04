using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Planta.Commands;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class DeletePlantaCommandHandler : IRequestHandler<DeletePlantaCommand, bool>
{
    private readonly IPlantaRepository _plantaRepository;
    private readonly IAuthorizationService _authorizationService;

    public DeletePlantaCommandHandler(IPlantaRepository plantaRepository, IAuthorizationService authorizationService)
    {
        _plantaRepository = plantaRepository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(DeletePlantaCommand request, CancellationToken cancellationToken)
    {
        var planta = await _plantaRepository.GetByIdAsync(request.Id);
        
        if (planta == null)
            return false;

        var eliminadoPorIdString = _authorizationService.GetCurrentUserId();
        var eliminadoPorId = Guid.Parse(eliminadoPorIdString);

        await _plantaRepository.DeleteAsync(request.Id, eliminadoPorId);
        
        return true;
    }
}
