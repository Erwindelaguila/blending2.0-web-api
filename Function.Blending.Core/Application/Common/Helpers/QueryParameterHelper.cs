using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.AppParam.DTOs;
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
        
        filter.Codigo = query["codigo"];
        filter.Estado = query["estado"];
        
        // FechaDesde
        if (!string.IsNullOrWhiteSpace(query["fechaDesde"]) &&
            DateTime.TryParseExact(query["fechaDesde"], "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var fechaDesde))
        {
            filter.FechaDesde = fechaDesde;
        }

        return filter;
    }

    public static LineaProduccionFilterDTO ParseLineaProduccionFilters(NameValueCollection query)
    {
        var filter = new LineaProduccionFilterDTO();
        
        filter.Codigo = query["codigo"];
        filter.Estado = query["estado"];
        
        // FechaDesde
        if (!string.IsNullOrWhiteSpace(query["fechaDesde"]) &&
            DateTime.TryParseExact(query["fechaDesde"], "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var fechaDesde))
        {
            filter.FechaDesde = fechaDesde;
        }

        return filter;
    }

    public static PlantaFilterDTO ParsePlantaFilters(NameValueCollection query)
    {
        var filter = new PlantaFilterDTO();
        
        filter.Codigo = query["codigo"];
        filter.Estado = query["estado"];
        filter.IsHarina = query["isHarina"];
        
        // FechaDesde
        if (!string.IsNullOrWhiteSpace(query["fechaDesde"]) &&
            DateTime.TryParseExact(query["fechaDesde"], "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var fechaDesde))
        {
            filter.FechaDesde = fechaDesde;
        }

        return filter;
    }

    public static ParametroFilterDTO ParseParametroFilters(NameValueCollection query)
    {
        var filter = new ParametroFilterDTO();
        
        filter.Codigo = query["codigo"];
        filter.Estado = query["estado"];
        
        // FechaDesde
        if (!string.IsNullOrWhiteSpace(query["fechaDesde"]) &&
            DateTime.TryParseExact(query["fechaDesde"], "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var fechaDesde))
        {
            filter.FechaDesde = fechaDesde;
        }

        return filter;
    }

    public static CalidadFilterDTO ParseCalidadFilters(NameValueCollection query)
    {
        var filter = new CalidadFilterDTO();
        
        filter.Codigo = query["codigo"];
        filter.Estado = query["estado"];
        
        // FechaDesde
        if (!string.IsNullOrWhiteSpace(query["fechaDesde"]) &&
            DateTime.TryParseExact(query["fechaDesde"], "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var fechaDesde))
        {
            filter.FechaDesde = fechaDesde;
        }

        return filter;
    }

    public static TipoProduccionFilterDTO ParseTipoProduccionFilters(NameValueCollection query)
    {
        var filter = new TipoProduccionFilterDTO();
        
        filter.Codigo = query["codigo"];
        filter.Estado = query["estado"];
        
        // FechaDesde
        if (!string.IsNullOrWhiteSpace(query["fechaDesde"]) &&
            DateTime.TryParseExact(query["fechaDesde"], "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var fechaDesde))
        {
            filter.FechaDesde = fechaDesde;
        }

        return filter;
    }

    public static ProductoFilterDTO ParseProductoFilters(NameValueCollection query)
    {
        var filter = new ProductoFilterDTO();
        
        filter.Codigo = query["codigo"];
        filter.Estado = query["estado"];
        
        // FechaDesde
        if (!string.IsNullOrWhiteSpace(query["fechaDesde"]) &&
            DateTime.TryParseExact(query["fechaDesde"], "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var fechaDesde))
        {
            filter.FechaDesde = fechaDesde;
        }
        
        if (Guid.TryParse(query["calidadId"], out var calidadId))
            filter.CalidadId = calidadId;
            
        if (Guid.TryParse(query["tipoProduccionId"], out var tipoProduccionId))
            filter.TipoProduccionId = tipoProduccionId;

        return filter;
    }

    public static AppParamFilterDTO ParseAppParamFilters(NameValueCollection query)
    {
        var filter = new AppParamFilterDTO();
        
        filter.Key = query["key"];
        
        if (bool.TryParse(query["isActive"], out var isActive))
            filter.IsActive = isActive;
        
        // FechaDesde
        if (!string.IsNullOrWhiteSpace(query["fechaDesde"]) &&
            DateTime.TryParseExact(query["fechaDesde"], "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var fechaDesde))
        {
            filter.FechaDesde = fechaDesde;
        }

        return filter;
    }

    public static bool HasActiveFilters(AgregadoFilterDTO filter) => 
        !string.IsNullOrWhiteSpace(filter.Codigo) || 
        !string.IsNullOrWhiteSpace(filter.Estado) || 
        filter.FechaDesde.HasValue;

    public static bool HasActiveFilters(AppParamFilterDTO filter) => 
        !string.IsNullOrWhiteSpace(filter.Key) || 
        filter.IsActive.HasValue || 
        filter.FechaDesde.HasValue;

    public static bool HasActiveFilters(LineaProduccionFilterDTO filter) => 
        !string.IsNullOrWhiteSpace(filter.Codigo) || 
        !string.IsNullOrWhiteSpace(filter.Estado) || 
        filter.FechaDesde.HasValue;

    public static bool HasActiveFilters(PlantaFilterDTO filter) => 
        !string.IsNullOrWhiteSpace(filter.Codigo) || 
        !string.IsNullOrWhiteSpace(filter.Estado) || 
        !string.IsNullOrWhiteSpace(filter.IsHarina) ||
        filter.FechaDesde.HasValue;

    public static bool HasActiveFilters(ParametroFilterDTO filter) => 
        !string.IsNullOrWhiteSpace(filter.Codigo) || 
        !string.IsNullOrWhiteSpace(filter.Estado) || 
        filter.FechaDesde.HasValue;

    public static bool HasActiveFilters(CalidadFilterDTO filter) => 
        !string.IsNullOrWhiteSpace(filter.Codigo) || 
        !string.IsNullOrWhiteSpace(filter.Estado) || 
        filter.FechaDesde.HasValue;

    public static bool HasActiveFilters(TipoProduccionFilterDTO filter) => 
        !string.IsNullOrWhiteSpace(filter.Codigo) || 
        !string.IsNullOrWhiteSpace(filter.Estado) || 
        filter.FechaDesde.HasValue;

    public static bool HasActiveFilters(ProductoFilterDTO filter) => 
        !string.IsNullOrWhiteSpace(filter.Codigo) || 
        !string.IsNullOrWhiteSpace(filter.Estado) || 
        filter.CalidadId.HasValue || 
        filter.TipoProduccionId.HasValue || 
        filter.FechaDesde.HasValue;
}
