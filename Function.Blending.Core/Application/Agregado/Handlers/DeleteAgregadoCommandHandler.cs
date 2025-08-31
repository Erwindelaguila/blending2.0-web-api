using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Agregado.Commands;
using Function.Blending.Core.Application.Common.Exceptions;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers;

public class DeleteAgregadoCommandHandler : IRequestHandler<DeleteAgregadoCommand, bool>
{
    private readonly IAgregadoRepository _agregadoRepository;
    private readonly IAuthorizationService _authorizationService;

    public DeleteAgregadoCommandHandler(
        IAgregadoRepository agregadoRepository,
        IAuthorizationService authorizationService)
    {
        _agregadoRepository = agregadoRepository;
        _authorizationService = authorizationService;
    }    public async Task<bool> Handle(DeleteAgregadoCommand request, CancellationToken cancellationToken)
    {
        var agregado = await _agregadoRepository.GetByIdAsync(request.Id);
        if (agregado == null)
            return false;

        var isUsedByActiveTipoProduccion = await _agregadoRepository.IsUsedByActiveTipoProduccionAsync(request.Id);
        if (isUsedByActiveTipoProduccion)
        {
            throw new EntityInUseException("el Agregado", "está siendo usado por al menos un Tipo de Producción activo");
        }
        var currentUserIdString = _authorizationService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            throw new UnauthorizedAccessException("User ID inválido en headers");
        }

        try
        {
            await _agregadoRepository.DeleteAsync(request.Id, currentUserId);
            return true;
        }
        catch (Exception)
        {
            throw new EntityInUseException("el Agregado", "tiene dependencias en la base de datos");
        }
    }
}
