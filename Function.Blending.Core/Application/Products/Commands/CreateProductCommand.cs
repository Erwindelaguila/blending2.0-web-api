using Function.Blending.Core.Application.Products.DTOs;
using MediatR;
namespace Function.Blending.Core.Application.Products.Commands;

public class CreateProductCommand : IRequest<ProductDto>
{
    public string Codigo { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string Descripcion { get; set; } = default!;
    public Guid CalidadId { get; set; }
    public Guid TipoProduccionId { get; set; }
    public Guid CreadoPorId { get; set; }
}