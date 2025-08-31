using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Producto.Queries;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Handlers;

public class GetProductoByIdQueryHandler : IRequestHandler<GetProductoByIdQuery, ProductoDTO?>
{
    private readonly IProductoRepository _productoRepository;

    public GetProductoByIdQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<ProductoDTO?> Handle(GetProductoByIdQuery request, CancellationToken cancellationToken)
    {
        return await _productoRepository.GetByIdWithRelationsAsync(request.Id);
    }
}
