using System;

namespace Function.Blending.Core.Application.Producto.DTOs;

public class ProductoDTO
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public CalidadRelacion Calidad { get; set; } = null!;
    public TipoProduccionRelacion TipoProduccion { get; set; } = null!;
    public bool Activo { get; set; }
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
}

public class CalidadRelacion
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
}

public class TipoProduccionRelacion
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
}
