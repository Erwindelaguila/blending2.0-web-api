using MediatR;
using Function.Blending.Core.Application.TipoProduccion.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;
using Function.Blending.Core.Application.Common.Exceptions;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;


public class DeleteTipoProduccionCommandHandler : IRequestHandler<DeleteTipoProduccionCommand, bool>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;
    
    public DeleteTipoProduccionCommandHandler(
        ITipoProduccionRepository tipoProduccionRepository,
        IFunctionContextAccessor functionContextAccessor)
    {
        _tipoProduccionRepository = tipoProduccionRepository ?? throw new ArgumentNullException(nameof(tipoProduccionRepository));
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<bool> Handle(DeleteTipoProduccionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();

        var tipo = await _tipoProduccionRepository.GetByIdAsync(request.Id);
        if (tipo == null)
            return false;

        try
        {
            var isUsedByActiveProducto = await _tipoProduccionRepository.IsUsedByActiveProductoAsync(request.Id);
            if (isUsedByActiveProducto)
            {
                throw new EntityInUseException("el Tipo de Producción", "está siendo usado por al menos un Producto activo");
            }

            await _tipoProduccionRepository.DeleteAsync(request.Id, currentUserId); 
            return true;
        }
        catch (Exception)
        {
           
            return false;
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
