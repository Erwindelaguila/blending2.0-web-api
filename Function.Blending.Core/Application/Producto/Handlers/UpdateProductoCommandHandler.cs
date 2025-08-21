using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Producto.Commands;
using Function.Blending.Core.Application.Producto.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Handlers;

public class UpdateProductoCommandHandler : IRequestHandler<UpdateProductoCommand, ProductoDTO>
{
    private readonly IProductoRepository _productoRepository;
    public UpdateProductoCommandHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<ProductoDTO> Handle(UpdateProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.GetByIdAsync(request.Id);
        if (producto == null)
            throw new ArgumentException($"Producto con ID {request.Id} no encontrado");

        producto.Codigo = request.Codigo ?? producto.Codigo;
        producto.Nombre = request.Nombre ?? producto.Nombre;
        producto.Descripcion = request.Descripcion ?? producto.Descripcion;
        producto.CalidadId = request.CalidadId ?? producto.CalidadId;
        producto.TipoProduccionId = request.TipoProduccionId ?? producto.TipoProduccionId;
        producto.Activo = request.Activo ?? producto.Activo;
        producto.ModificadoPorId = request.ModificadoPorId;
        producto.ModificadoEl = DateTime.UtcNow;

        await _productoRepository.UpdateAsync(producto);

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
