using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Parametro.Commands;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class DeleteParametroCommandHandler : IRequestHandler<DeleteParametroCommand, bool>
{
    private readonly IParametroRepository _parametroRepository;
    private readonly IAuthorizationService _authorizationService;

    public DeleteParametroCommandHandler(IParametroRepository parametroRepository, IAuthorizationService authorizationService)
    {
        _parametroRepository = parametroRepository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(DeleteParametroCommand request, CancellationToken cancellationToken)
    {
        var parametro = await _parametroRepository.GetByIdAsync(request.Id);
        
        if (parametro == null)
            return false;

        var eliminadoPorIdString = _authorizationService.GetCurrentUserId();
        var eliminadoPorId = Guid.Parse(eliminadoPorIdString);

        await _parametroRepository.DeleteAsync(request.Id, eliminadoPorId);
        
        return true;
    }
}
