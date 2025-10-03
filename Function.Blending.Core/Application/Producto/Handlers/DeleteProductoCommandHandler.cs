using MediatR;
using Function.Blending.Core.Application.Producto.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;

using Function.Blending.Core.Application.Common.Exceptions;

namespace Function.Blending.Core.Application.Producto.Handlers;


public class DeleteProductoCommandHandler : IRequestHandler<DeleteProductoCommand, bool>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;
    
    public DeleteProductoCommandHandler(
        IProductoRepository productoRepository,
        IFunctionContextAccessor functionContextAccessor)
    {
        _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<bool> Handle(DeleteProductoCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();

        var producto = await _productoRepository.GetByIdAsync(request.Id);
        if (producto == null)
            return false;

        try
        {
            await _productoRepository.DeleteAsync(request.Id, currentUserId); 
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
