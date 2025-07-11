namespace Function.Blending.Core.Domain.Entities;

public class Producto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public Guid CalidadId { get; set; }
    public Guid TipoProduccionId { get; set; }
    public bool Activo { get; set; }
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
    
    public Calidad? Calidad { get; set; }
    public TipoProduccion? TipoProduccion { get; set; }
}