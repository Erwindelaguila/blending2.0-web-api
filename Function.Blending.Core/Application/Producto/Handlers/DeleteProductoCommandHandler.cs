using MediatR;
using Function.Blending.Core.Application.Producto.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Common.Exceptions;

namespace Function.Blending.Core.Application.Producto.Handlers;


public class DeleteProductoCommandHandler : IRequestHandler<DeleteProductoCommand, bool>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IAuthorizationService _authorizationService;
    
    public DeleteProductoCommandHandler(
        IProductoRepository productoRepository,
        IAuthorizationService authorizationService)
    {
        _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
    }

    public async Task<bool> Handle(DeleteProductoCommand request, CancellationToken cancellationToken)
    {
        var currentUserIdString = _authorizationService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            throw new UnauthorizedAccessException("User ID inválido en headers");
        }

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
}
