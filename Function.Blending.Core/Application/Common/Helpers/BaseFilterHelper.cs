using Function.Blending.Core.Application.Common.Interfaces;
using System.Collections.Specialized;
using System.Globalization;

namespace Function.Blending.Core.Application.Common.Helpers;

public static class BaseFilterHelper
{
    /// <summary>
    /// Aplica filtros base (código, estado, fechas) a cualquier filtro que implemente IBaseEntityFilter
    /// </summary>
    /// <typeparam name="T">Tipo de filtro que implementa IBaseEntityFilter</typeparam>
    /// <param name="filter">Instancia del filtro</param>
    /// <param name="query">Parámetros de query</param>
    /// <returns>El filtro poblado con los valores base</returns>
    public static T PopulateBaseFilters<T>(T filter, NameValueCollection query) where T : IBaseEntityFilter
    {
        // Filtro por código
        if (!string.IsNullOrWhiteSpace(query["codigo"]))
        {
            filter.Codigo = query["codigo"]!.Trim();
        }

        // Filtro por estado - CONTRATO ESTRICTO PARA PRODUCCIÓN
        if (!string.IsNullOrWhiteSpace(query["estado"]))
        {
            var estado = query["estado"]!.Trim();
            if (estado == "1" || estado == "0")
            {
                filter.Estado = estado;
            }
            else
            {
                throw new ArgumentException($"Parámetro 'estado' inválido: '{estado}'. Use '1' para activos o '0' para inactivos.");
            }
        }

        // Filtro por fecha desde (acepta fechaDesde o fechaInicio)
        var fechaDesdeParam = query["fechaDesde"] ?? query["fechaInicio"];
        if (!string.IsNullOrWhiteSpace(fechaDesdeParam))
        {
            if (DateTime.TryParseExact(fechaDesdeParam, "yyyy-MM-dd", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaDesde))
            {
                filter.FechaDesde = fechaDesde;
            }
        }

        // Filtro por fecha hasta (acepta fechaHasta o fechaFin)
        var fechaHastaParam = query["fechaHasta"] ?? query["fechaFin"];
        if (!string.IsNullOrWhiteSpace(fechaHastaParam))
        {
            if (DateTime.TryParseExact(fechaHastaParam, "yyyy-MM-dd", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaHasta))
            {
                filter.FechaHasta = fechaHasta;
            }
        }

        return filter;
    }

    /// <summary>
    /// Valida si el filtro base tiene al menos un parámetro válido
    /// </summary>
    /// <param name="filter">Filtro a validar</param>
    /// <returns>True si tiene al menos un filtro base activo</returns>
    public static bool HasBaseActiveFilters(IBaseEntityFilter filter)
    {
        return !string.IsNullOrWhiteSpace(filter.Codigo) ||
               !string.IsNullOrWhiteSpace(filter.Estado) ||
               filter.FechaDesde.HasValue ||
               filter.FechaHasta.HasValue;
    }
}
