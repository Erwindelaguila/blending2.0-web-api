using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Producto.Commands;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Handlers;

public class DeleteProductoCommandHandler : IRequestHandler<DeleteProductoCommand, BaseResponse<object>>
{
    private readonly IProductoRepository _productoRepository;
    
    public DeleteProductoCommandHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<BaseResponse<object>> Handle(DeleteProductoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var producto = await _productoRepository.GetByIdAsync(request.Id);
            
            if (producto == null)
                return BaseResponse<object>.Fail("Producto no encontrado", null, 404);

            await _productoRepository.DeleteAsync(request.Id, request.EliminadoPorId);
            
            return BaseResponse<object>.Success(new { }, "Producto eliminado correctamente");
        }
        catch (Exception ex)
        {
            return BaseResponse<object>.Fail($"Error al eliminar el producto: {ex.Message}", null, 500);
        }
    }
}
