using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Calidad.Commands;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class DeleteCalidadCommandHandler : IRequestHandler<DeleteCalidadCommand, bool>
{
    private readonly ICalidadRepository _calidadRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public DeleteCalidadCommandHandler(
        ICalidadRepository calidadRepository,
        IFunctionContextAccessor functionContextAccessor)
    {
        _calidadRepository = calidadRepository;
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<bool> Handle(DeleteCalidadCommand request, CancellationToken cancellationToken)
    {
        var calidad = await _calidadRepository.GetByIdAsync(request.Id);
        
        if (calidad == null)
            return false;

        var isUsedByActiveProducto = await _calidadRepository.IsUsedByActiveProductoAsync(request.Id);
        if (isUsedByActiveProducto)
        {
            throw new EntityInUseException("la Calidad", "está siendo usada por al menos un Producto activo");
        }

        var eliminadoPorId = GetCurrentUserId();

        await _calidadRepository.DeleteAsync(request.Id, eliminadoPorId);
        
        return true;
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
