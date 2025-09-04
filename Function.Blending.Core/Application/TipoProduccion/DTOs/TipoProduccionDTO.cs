namespace Function.Blending.Core.Application.TipoProduccion.DTOs;

public class TipoProduccionDTO
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public LineaProduccionRelacion LineaProduccion { get; set; } = null!;
    public AgregadoRelacion Agregado { get; set; } = null!;
    public bool Activo { get; set; }
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
}

public class LineaProduccionRelacion
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
}

public class AgregadoRelacion
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
}
