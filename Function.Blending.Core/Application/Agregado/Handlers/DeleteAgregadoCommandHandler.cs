using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Agregado.Commands;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers;

public class DeleteAgregadoCommandHandler : IRequestHandler<DeleteAgregadoCommand, bool>
{
    private readonly IAgregadoRepository _agregadoRepository;
    public DeleteAgregadoCommandHandler(IAgregadoRepository agregadoRepository)
    {
        _agregadoRepository = agregadoRepository;
    }
    public async Task<bool> Handle(DeleteAgregadoCommand request, CancellationToken cancellationToken)
    {
        var agregado = await _agregadoRepository.GetByIdAsync(request.Id);

        if (agregado == null)
            return false;

        await _agregadoRepository.DeleteAsync(request.Id);

        return true;
    }
}
