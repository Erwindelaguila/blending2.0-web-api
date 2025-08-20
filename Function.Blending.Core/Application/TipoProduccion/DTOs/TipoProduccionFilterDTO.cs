using Function.Blending.Core.Application.Common.Interfaces;

namespace Function.Blending.Core.Application.TipoProduccion.DTOs;

public class TipoProduccionFilterDTO : IBaseEntityFilter
{
    public string? Codigo { get; set; }
    public string? Estado { get; set; }
    public DateTime? FechaDesde { get; set; }
}
