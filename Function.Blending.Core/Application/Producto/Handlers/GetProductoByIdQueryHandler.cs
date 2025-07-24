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
        var producto = await _productoRepository.GetByIdAsync(request.Id);
        if (producto == null)
            return null;
        return new ProductoDTO
        {
            Id = producto.Id,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            CalidadId = producto.CalidadId,
            TipoProduccionId = producto.TipoProduccionId,
            Activo = producto.Activo,
            CreadoPorId = producto.CreadoPorId,
            CreadoEl = producto.CreadoEl,
            ModificadoPorId = producto.ModificadoPorId,
            ModificadoEl = producto.ModificadoEl
        };
    }
}
