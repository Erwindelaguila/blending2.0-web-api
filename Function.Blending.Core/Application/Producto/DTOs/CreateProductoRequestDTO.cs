using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Producto.DTOs;


public class CreateProductoRequestDTO
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public Guid CalidadId { get; set; }
    public Guid TipoProduccionId { get; set; }
    public bool? Activo { get; set; } = true;
}
