using MediatR;
using Function.Blending.Core.Application.Producto.DTOs;

namespace Function.Blending.Core.Application.Producto.Queries;

public class GetAllProductosQuery : IRequest<List<ProductoDTO>>;
