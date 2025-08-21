using MediatR;
using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;

public class DeleteCalidadCommandHandler : IRequestHandler<DeleteCalidadCommand, bool>
{
    private readonly ICalidadRepository _calidadRepository;

    public DeleteCalidadCommandHandler(ICalidadRepository calidadRepository)
    {
        _calidadRepository = calidadRepository;
    }

    public async Task<bool> Handle(DeleteCalidadCommand request, CancellationToken cancellationToken)
    {
        var entity = await _calidadRepository.GetByIdAsync(request.Id);
        if (entity == null)
            return false;
            
        await _calidadRepository.DeleteAsync(request.Id, request.EliminadoPorId);
        return true;
    }
}
