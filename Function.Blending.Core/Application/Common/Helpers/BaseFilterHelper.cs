using Function.Blending.Core.Application.Common.Interfaces;
using System.Collections.Specialized;
using System.Globalization;

namespace Function.Blending.Core.Application.Common.Helpers;

public static class BaseFilterHelper
{
    public static T PopulateBaseFilters<T>(T filter, NameValueCollection query) where T : IBaseEntityFilter
    {
        if (!string.IsNullOrWhiteSpace(query["codigo"]))
        {
            filter.Codigo = query["codigo"]!.Trim();
        }

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
        
        

        var fechaDesdeParam = query["fechaDesde"];
        if (!string.IsNullOrWhiteSpace(fechaDesdeParam))
        {
            if (DateTime.TryParseExact(fechaDesdeParam, "yyyy-MM-dd", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaDesde))
            {
                filter.FechaDesde = fechaDesde;
            }
        }

        return filter;
    }

    public static bool HasBaseActiveFilters(IBaseEntityFilter filter)
    {
        return !string.IsNullOrWhiteSpace(filter.Codigo) ||
               !string.IsNullOrWhiteSpace(filter.Estado) ||
               filter.FechaDesde.HasValue;
    }
}
