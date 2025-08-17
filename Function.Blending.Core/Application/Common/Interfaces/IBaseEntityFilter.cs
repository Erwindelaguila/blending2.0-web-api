namespace Function.Blending.Core.Application.Common.Interfaces;

/// <summary>
/// Interfaz base para filtros de entidades que contienen las propiedades comunes
/// </summary>
public interface IBaseEntityFilter
{
    /// <summary>
    /// Filtro por código (modo prefijo por defecto para aprovechar índices)
    /// </summary>
    string? Codigo { get; set; }

    /// <summary>
    /// Filtro por estado: "activo" | "inactivo"
    /// </summary>
    string? Estado { get; set; }

    /// <summary>
    /// Fecha de inicio para filtro por rango (inclusive)
    /// </summary>
    DateTime? FechaDesde { get; set; }

    /// <summary>
    /// Fecha de fin para filtro por rango (inclusive)
    /// </summary>
    DateTime? FechaHasta { get; set; }
}
