using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Agregado.Commands;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Shared.Extensions;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Functions.Support.Execution;
using System.Security.Claims;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers;

public class DeleteAgregadoCommandHandler : IRequestHandler<DeleteAgregadoCommand, bool>
{
    private readonly IAgregadoRepository _agregadoRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public DeleteAgregadoCommandHandler(IAgregadoRepository agregadoRepository, IFunctionContextAccessor functionContextAccessor)
    {
        _agregadoRepository = agregadoRepository;
        _functionContextAccessor = functionContextAccessor;
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
        
        var currentUserId = GetCurrentUserId(); 

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

    private Guid GetCurrentUserId()
    {
        try
        {
            var context = _functionContextAccessor.Current;
            if (context?.Items.TryGetValue(MiscellaneousConstants.Principal, out var principalObj) == true &&
                principalObj is ClaimsPrincipal principal)
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
