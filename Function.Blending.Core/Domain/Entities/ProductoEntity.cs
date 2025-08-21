namespace Function.Blending.Core.Domain.Entities;

public class ProductoEntity
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public Guid CalidadId { get; set; }
    public Guid TipoProduccionId { get; set; }
    public bool Activo { get; set; }
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
    public bool Eliminado { get; set; }
    public Guid? EliminadoPorId { get; set; }
    public DateTime? EliminadoEl { get; set; }
    
    public CalidadEntity? Calidad { get; set; }
    public TipoProduccionEntity? TipoProduccion { get; set; }
}