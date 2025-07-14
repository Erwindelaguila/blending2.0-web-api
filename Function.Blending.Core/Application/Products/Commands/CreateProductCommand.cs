using Function.Blending.Core.Application.Products.DTOs;
using MediatR;
namespace Function.Blending.Core.Application.Products.Commands;

public class CreateProductCommand : IRequest<ProductDto>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string Descripcion { get; }
    public Guid CalidadId { get; }
    public Guid TipoProduccionId { get; }
    public Guid CreadoPorId { get; }

    public CreateProductCommand(
        string codigo,
        string nombre,
        string descripcion,
        Guid calidadId,
        Guid tipoProduccionId,
        Guid creadoPorId)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        CalidadId = calidadId;
        TipoProduccionId = tipoProduccionId;
        CreadoPorId = creadoPorId;
    }
}