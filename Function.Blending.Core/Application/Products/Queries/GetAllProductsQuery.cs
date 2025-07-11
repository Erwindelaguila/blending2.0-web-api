using Function.Blending.Core.Application.Products.DTOs;
using MediatR;
namespace Function.Blending.Core.Application.Products.Queries;

public record GetAllProductsQuery() : IRequest<List<ProductDto>>;
