using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Parametro.Commands;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class DeleteParametroCommandHandler : IRequestHandler<DeleteParametroCommand, bool>
{
    private readonly IParametroRepository _parametroRepository;

    public DeleteParametroCommandHandler(IParametroRepository parametroRepository)
    {
        _parametroRepository = parametroRepository;
    }

    public async Task<bool> Handle(DeleteParametroCommand request, CancellationToken cancellationToken)
    {
        var parametro = await _parametroRepository.GetByIdAsync(request.Id);
        
        if (parametro == null)
            return false;

        await _parametroRepository.DeleteAsync(request.Id, request.EliminadoPorId);
        
        return true;
    }
}
