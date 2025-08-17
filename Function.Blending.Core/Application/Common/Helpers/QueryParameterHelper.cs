using Function.Blending.Core.Application.Agregado.DTOs;
using System.Collections.Specialized;

namespace Function.Blending.Core.Application.Common.Helpers;

public static class QueryParameterHelper
{
    public static AgregadoFilterDTO ParseAgregadoFilters(NameValueCollection query)
    {
        var filter = new AgregadoFilterDTO();
        BaseFilterHelper.PopulateBaseFilters(filter, query);
        if (!string.IsNullOrWhiteSpace(query["tipoFecha"]))
        {
            var tipoFecha = query["tipoFecha"]!.Trim().ToLowerInvariant();
            if (tipoFecha == "todos" || tipoFecha == "creados" || tipoFecha == "modificados")
                filter.TipoFecha = tipoFecha;
        }
        if (string.IsNullOrWhiteSpace(filter.TipoFecha) && (filter.FechaDesde.HasValue || filter.FechaHasta.HasValue))
            filter.TipoFecha = "creados";
        return filter;
    }
    public static bool HasActiveFilters(AgregadoFilterDTO filter) => BaseFilterHelper.HasBaseActiveFilters(filter);
}
