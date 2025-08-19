using Function.Blending.Core.Application.Common.Interfaces;

namespace Function.Blending.Core.Application.Agregado.DTOs;

public class AgregadoFilterDTO : IBaseEntityFilter
{
    public string? Codigo { get; set; }
    public string? Estado { get; set; }
    public DateTime? FechaDesde { get; set; }
}
