namespace Function.Blending.Core.Domain.Entities;

public class TipoProduccionEntity
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public Guid LineaProduccionId { get; set; }
    public Guid AgregadoId { get; set; }
    public bool Activo { get; set; }
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }

    public AgregadoEntity? Agregado { get; set; }
    public LineaProduccionEntity? LineaProduccion { get; set; }
}
