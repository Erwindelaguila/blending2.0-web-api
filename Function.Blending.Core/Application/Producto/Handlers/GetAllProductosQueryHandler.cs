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
        var entities = await _productoRepository.GetAllAsync();
        return entities.Select(entity => new ProductoDTO
        {
            Id = entity.Id,
            Codigo = entity.Codigo,
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            CalidadId = entity.CalidadId,
            TipoProduccionId = entity.TipoProduccionId,
            Activo = entity.Activo,
            CreadoPorId = entity.CreadoPorId,
            CreadoEl = entity.CreadoEl,
            ModificadoPorId = entity.ModificadoPorId,
            ModificadoEl = entity.ModificadoEl
        }).ToList();
    }
}
