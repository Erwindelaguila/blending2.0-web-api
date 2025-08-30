namespace Function.Blending.Core.Application.Producto.DTOs;

public class ProductoFilterDTO
{
    public string? Codigo { get; set; }
    public string? Estado { get; set; }
    public DateTime? FechaDesde { get; set; }
    public Guid? CalidadId { get; set; }
    public Guid? TipoProduccionId { get; set; }
    
}
