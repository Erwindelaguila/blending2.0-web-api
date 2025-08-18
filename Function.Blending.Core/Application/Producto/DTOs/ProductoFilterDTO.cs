using Function.Blending.Core.Application.Common.Interfaces;

namespace Function.Blending.Core.Application.Producto.DTOs;

public class ProductoFilterDTO : IBaseEntityFilter
{
    public string? Codigo { get; set; }
    public string? Estado { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public string? TipoFecha { get; set; }
}
