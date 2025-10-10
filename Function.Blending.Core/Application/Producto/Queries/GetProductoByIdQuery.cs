using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Common.Queries;

namespace Function.Blending.Core.Application.Producto.Queries;

public class GetProductoByIdQuery : BaseQuery<ProductoDTO?>
{
    public Guid Id { get; }

    public GetProductoByIdQuery(Guid id)
    {
        Id = id;
    }
}
