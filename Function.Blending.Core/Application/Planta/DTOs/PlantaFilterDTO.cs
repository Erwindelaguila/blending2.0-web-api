using Function.Blending.Core.Application.Common.Interfaces;

namespace Function.Blending.Core.Application.Planta.DTOs;

public class PlantaFilterDTO : IBaseEntityFilter
{
    public string? Codigo { get; set; }
    public string? Estado { get; set; }
    public DateTime? FechaDesde { get; set; }
    public string? IsHarina { get; set; }
}
