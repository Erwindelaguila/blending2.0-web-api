namespace Function.Blending.Core.Application.Products.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public bool Activo { get; set; }
    public Guid CalidadId { get; set; }
    public string CalidadNombre { get; set; } = null!;
    public Guid TipoProduccionId { get; set; }
    public string TipoProduccionNombre { get; set; } = null!;
    
}