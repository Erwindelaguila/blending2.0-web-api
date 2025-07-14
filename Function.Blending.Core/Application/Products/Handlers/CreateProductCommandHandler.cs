using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Products.Commands;
using Function.Blending.Core.Application.Products.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Products.Handlers;


public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductoRepository _repository;

    public CreateProductCommandHandler(IProductoRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Mapeo a mano
        var producto = new ProductoEntity()
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            CalidadId = request.CalidadId,
            TipoProduccionId = request.TipoProduccionId,
            Activo = true,
            CreadoPorId = request.CreadoPorId,
            CreadoEl = DateTime.UtcNow
        };

        await _repository.AddAsync(producto);

        // Mapeo a mano también para el DTO
        return new ProductDto
        {
            Id = producto.Id,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            CalidadId = producto.CalidadId,
            TipoProduccionId = producto.TipoProduccionId,
            Activo = producto.Activo
        };
    }
}