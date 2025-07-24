using MediatR;
using Function.Blending.Core.Application.Producto.DTOs;

namespace Function.Blending.Core.Application.Producto.Queries;

public class GetProductoByIdQuery : IRequest<ProductoDTO?>
{
    public Guid Id { get; }

    public GetProductoByIdQuery(Guid id)
    {
        Id = id;
    }
}
