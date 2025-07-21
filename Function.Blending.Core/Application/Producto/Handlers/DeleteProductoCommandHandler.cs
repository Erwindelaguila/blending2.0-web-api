using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Producto.Commands;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Handlers;

public class DeleteProductoCommandHandler : IRequestHandler<DeleteProductoCommand, bool>
{
    private readonly IProductoRepository _productoRepository;
    public DeleteProductoCommandHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<bool> Handle(DeleteProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.GetByIdAsync(request.Id);
        if (producto == null)
            return false;
        await _productoRepository.DeleteAsync(request.Id);
        return true;
    }
}
