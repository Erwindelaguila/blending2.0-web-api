namespace Function.Blending.Core.Domain.Entities;

public class CalidadParametroEntity
{
    public Guid Id { get; set; }
    public Guid CalidadId { get; set; }
    public Guid ParametroId { get; set; }
    public decimal Valor { get; set; }
    public bool Activo { get; set; } = true;
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; } = DateTime.UtcNow;
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
    
    // Navigation properties
    public CalidadEntity? Calidad { get; set; }
    public ParametroEntity? Parametro { get; set; }
}
