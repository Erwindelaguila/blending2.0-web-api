using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Queries;

public class GetAllProductoWithPaginationQuery : IRequest<PagedResponse<ProductoDTO>>
{
    public int Page { get; }
    public int Size { get; }
    public ProductoFilterDTO? Filters { get; }

    public GetAllProductoWithPaginationQuery(int page, int size, ProductoFilterDTO? filters = null)
    {
        Page = page;
        Size = size;
        Filters = filters;
    }
}
