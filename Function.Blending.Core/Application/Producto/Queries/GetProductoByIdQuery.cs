using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Common.Queries;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Producto.Queries;

public class GetProductoByIdQuery : BaseQuery<ProductoDTO?>
{
    public Guid Id { get; }

    public GetProductoByIdQuery(Guid id, HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Id = id;
    }
}
