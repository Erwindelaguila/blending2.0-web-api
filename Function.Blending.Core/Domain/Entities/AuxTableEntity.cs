namespace Function.Blending.Core.Domain.Entities;

public class AuxTableEntity
{
    public Guid Id { get; set; }
    public Guid? PadreId { get; set; }
    public int Orden { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Clave { get; set; }
    public bool Activo { get; set; }
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
}