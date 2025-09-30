using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Application.LineaProduccion.Commands;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using Function.Blending.Core.Shared.Security;
using System.Security.Claims;
using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class DeleteLineaProduccionCommandHandler : IRequestHandler<DeleteLineaProduccionCommand, bool>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public DeleteLineaProduccionCommandHandler(ILineaProduccionRepository lineaProduccionRepository, IFunctionContextAccessor functionContextAccessor)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<bool> Handle(DeleteLineaProduccionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();

        var linea = await _lineaProduccionRepository.GetByIdAsync(request.Id);
        if (linea == null)
            return false;

        var isUsedByActiveTipoProduccion = await _lineaProduccionRepository.IsUsedByActiveTipoProduccionAsync(request.Id);
        if (isUsedByActiveTipoProduccion)
        {
            throw new EntityInUseException("la Línea de Producción", "está siendo usada por al menos un Tipo de Producción activo");
        }

        try
        {
            await _lineaProduccionRepository.DeleteAsync(request.Id, currentUserId);
            return true;
        }
        catch (Exception)
        {
            throw new EntityInUseException("la Línea de Producción", "tiene dependencias en la base de datos");
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
