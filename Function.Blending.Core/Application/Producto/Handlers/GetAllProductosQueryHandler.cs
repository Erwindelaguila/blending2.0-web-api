using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Producto.Queries;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Handlers;

public class GetAllProductosQueryHandler : IRequestHandler<GetAllProductosQuery, List<ProductoDTO>>
{
    private readonly IProductoRepository _productoRepository;
    public GetAllProductosQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<List<ProductoDTO>> Handle(GetAllProductosQuery request, CancellationToken cancellationToken)
    {
        return await _productoRepository.GetAllWithRelationsAsync();
    }
}
