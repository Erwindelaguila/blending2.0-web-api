using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Products.DTOs;
using Function.Blending.Core.Application.Products.Queries;
using MediatR;

namespace Function.Blending.Core.Application.Products.Handlers;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IProductoRepository _repository;

    public GetAllProductsQueryHandler(IProductoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var productos = await _repository.GetAllAsync();

        var productosDto = productos.Select(producto => new ProductDto
        {
            Id = producto.Id,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            CalidadId = producto.CalidadId,
            TipoProduccionId = producto.TipoProduccionId,
            Activo = producto.Activo,
            CalidadNombre = producto.Calidad?.Nombre ?? "",
            TipoProduccionNombre = producto.TipoProduccion?.Nombre ?? ""
        }).ToList();

        return productosDto;
    }
}