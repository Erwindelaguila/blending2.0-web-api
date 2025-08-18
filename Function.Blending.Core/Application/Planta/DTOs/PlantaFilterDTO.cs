using Function.Blending.Core.Application.Common.Interfaces;

namespace Function.Blending.Core.Application.Planta.DTOs;

public class PlantaFilterDTO : IBaseEntityFilter
{
    /// <summary>
    /// Filtro por código (modo prefijo por defecto para aprovechar índices)
    /// </summary>
    public string? Codigo { get; set; }

    /// <summary>
    /// Filtro por estado: "activo" | "inactivo"
    /// </summary>
    public string? Estado { get; set; }

    /// <summary>
    /// Fecha de inicio para filtro por rango (inclusive)
    /// </summary>
    public DateTime? FechaDesde { get; set; }

    /// <summary>
    /// Fecha de fin para filtro por rango (inclusive)
    /// </summary>
    public DateTime? FechaHasta { get; set; }

    /// <summary>
    /// Tipo de fecha para el filtro: "creados" | "modificados"
    /// </summary>
    public string? TipoFecha { get; set; }
}
