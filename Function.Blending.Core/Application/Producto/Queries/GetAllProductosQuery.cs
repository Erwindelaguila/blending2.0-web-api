using Function.Blending.Core.Application.Producto.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Queries;

public class GetAllProductosQuery : IRequest<List<ProductoDTO>>
{
}
