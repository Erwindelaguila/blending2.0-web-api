using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Planta.Commands;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class DeletePlantaCommandHandler : IRequestHandler<DeletePlantaCommand, bool>
{
    private readonly IPlantaRepository _plantaRepository;

    public DeletePlantaCommandHandler(IPlantaRepository plantaRepository)
    {
        _plantaRepository = plantaRepository;
    }

    public async Task<bool> Handle(DeletePlantaCommand request, CancellationToken cancellationToken)
    {
        var planta = await _plantaRepository.GetByIdAsync(request.Id);
        
        if (planta == null)
            return false;

        await _plantaRepository.DeleteAsync(request.Id);
        
        return true;
    }
}
