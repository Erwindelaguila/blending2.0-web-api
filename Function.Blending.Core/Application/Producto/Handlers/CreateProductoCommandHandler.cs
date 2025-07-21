using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Producto.Commands;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Handlers;

public class CreateProductoCommandHandler : IRequestHandler<CreateProductoCommand, ProductoDTO>
{
    private readonly IProductoRepository _productoRepository;
    public CreateProductoCommandHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<ProductoDTO> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = new ProductoEntity
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            CalidadId = request.CalidadId,
            TipoProduccionId = request.TipoProduccionId,
            Activo = request.Activo ?? true,
            CreadoPorId = request.CreadoPorId,
            CreadoEl = DateTime.UtcNow
        };
        await _productoRepository.CreateAsync(producto);
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
