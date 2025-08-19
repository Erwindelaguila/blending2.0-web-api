using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.Producto.DTOs;
using System.Collections.Specialized;

namespace Function.Blending.Core.Application.Common.Helpers;

public static class QueryParameterHelper
{
    public static AgregadoFilterDTO ParseAgregadoFilters(NameValueCollection query)
    {
        var filter = new AgregadoFilterDTO();
        BaseFilterHelper.PopulateBaseFilters(filter, query);
        return filter;
    }

    public static LineaProduccionFilterDTO ParseLineaProduccionFilters(NameValueCollection query)
    {
        var filter = new LineaProduccionFilterDTO();
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

    public static PlantaFilterDTO ParsePlantaFilters(NameValueCollection query)
    {
        var filter = new PlantaFilterDTO();
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

    public static ParametroFilterDTO ParseParametroFilters(NameValueCollection query)
    {
        var filter = new ParametroFilterDTO();
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

    public static CalidadFilterDTO ParseCalidadFilters(NameValueCollection query)
    {
        var filter = new CalidadFilterDTO();
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

    public static TipoProduccionFilterDTO ParseTipoProduccionFilters(NameValueCollection query)
    {
        var filter = new TipoProduccionFilterDTO();
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

    public static ProductoFilterDTO ParseProductoFilters(NameValueCollection query)
    {
        var filter = new ProductoFilterDTO();
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

    public static bool HasActiveFilters(AgregadoFilterDTO filter) => 
        BaseFilterHelper.HasBaseActiveFilters(filter);
    public static bool HasActiveFilters(LineaProduccionFilterDTO filter) => BaseFilterHelper.HasBaseActiveFilters(filter);
    public static bool HasActiveFilters(PlantaFilterDTO filter) => BaseFilterHelper.HasBaseActiveFilters(filter);
    public static bool HasActiveFilters(ParametroFilterDTO filter) => BaseFilterHelper.HasBaseActiveFilters(filter);
    public static bool HasActiveFilters(CalidadFilterDTO filter) => BaseFilterHelper.HasBaseActiveFilters(filter);
    public static bool HasActiveFilters(TipoProduccionFilterDTO filter) => BaseFilterHelper.HasBaseActiveFilters(filter);
    public static bool HasActiveFilters(ProductoFilterDTO filter) => BaseFilterHelper.HasBaseActiveFilters(filter);
}
