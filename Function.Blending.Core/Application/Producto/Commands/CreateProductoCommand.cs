using Function.Blending.Core.Application.Common.Commands;
using Function.Blending.Core.Application.Producto.DTOs;

namespace Function.Blending.Core.Application.Producto.Commands;

/// <summary>
/// Comando para crear productos
/// Implementa auditoría automática y validaciones de negocio
/// </summary>
public class CreateProductoCommand : BaseCommand<ProductoDTO>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public Guid CalidadId { get; }
    public Guid TipoProduccionId { get; }
    public bool? Activo { get; }

    public CreateProductoCommand(
        string codigo,
        string nombre,
        string? descripcion,
        Guid calidadId,
        Guid tipoProduccionId,
        bool? activo,
        object requestContext) : base(requestContext)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        CalidadId = calidadId;
        TipoProduccionId = tipoProduccionId;
        Activo = activo;
    }
}
