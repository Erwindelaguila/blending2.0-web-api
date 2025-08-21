using Function.Blending.Core.Application.Producto.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Queries;

public class GetAllProductoWithoutPaginationQuery : IRequest<List<ProductoDTO>>
{
    public ProductoFilterDTO? Filters { get; }

    public GetAllProductoWithoutPaginationQuery(ProductoFilterDTO? filters = null)
    {
        Filters = filters;
    }
}
