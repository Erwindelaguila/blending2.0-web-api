namespace Function.Blending.Core.Application.Agregado.DTOs;

using Function.Blending.Core.Application.Common.Interfaces;

public class AgregadoFilterDTO : IBaseEntityFilter
{
    // Reutilizamos convención base (codigo, estado, fechaDesde) + filtro específico opcional 'Desde' legado
    public string? Codigo { get; set; }
    public string? Estado { get; set; }
    public DateTime? FechaDesde { get; set; }
    // Campo previo (alias) para compatibilidad si se sigue usando 'desde'
    public DateTime? Desde { get; set; }
}
