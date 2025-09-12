using System.Globalization;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Models;
using Oferta = Function.Blending.Upload.Models.Oferta;

namespace Function.Blending.Upload.Helpers;

public static class ParsedRowMapperHelper
{
    public static RumaStockDisponibleDto Mapear(ParsedRowDto row, ExcelMappingConfig config,  List<CalidadDto> Calidades)
    {
        var result = new RumaStockDisponibleDto();

        // Tomamos el valor de RumaNro desde el row
        var rumaNro = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.RumaNro)) ?? string.Empty;
        var fechaFabricacion = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.FechaFabricacion)) ??
                               string.Empty;
        var fechaContabilizacion =
            ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.FechaContabilizacion)) ?? string.Empty;
        
        var anioCorto = rumaNro.Length >= 5 ? rumaNro.Substring(3, 2) : string.Empty;

        var anioCompleto = ObtenerAnioCompleto(anioCorto);
        var serie = rumaNro.Length >= 7 ? rumaNro.Substring(5, 2) : string.Empty;

        var codigo = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.Codigo)) ?? string.Empty;
        
        
        var descripcionCalidad = Calidades
            .FirstOrDefault(c => c.CodigoMaterial == codigo)?.Descripcion ?? string.Empty;
        
        
        var nombreCalidad = Calidades
            .FirstOrDefault(c => c.CodigoMaterial == codigo)?.Nombre ?? string.Empty;

        var fijos = new RumaValoresFijosDto
        {
            RumaNro = rumaNro,
            // Los 3 primeros caracteres (si hay al menos 3)
            Planta = rumaNro.Length >= 3 ? rumaNro.Substring(0, 3) : string.Empty,
            Anio = anioCorto,
            // Los caracteres 6 y 7 (posición 5 y 6) si hay al menos 7
            Serie = serie,
            FechaCorte = GetFechaCorte(serie,fechaFabricacion, anioCompleto, fechaContabilizacion),
            DescripcionCalidad = descripcionCalidad,
            NombreCalidad = nombreCalidad,
            Cantidad = ParsedRowValidator.ObtenerDouble(row.Fijos, nameof(config.Fijos.Cantidad)) ?? 0,
            Um = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.Um)) ?? string.Empty,
            Codigo = codigo,
            DescripcionMaterial = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.DescripcionMaterial)) ?? string.Empty,
            CentroUbicacion = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.CentroUbicacion)) ?? string.Empty,
            AlmacenUbicacion = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.AlmacenUbicacion)) ?? string.Empty,
            TipoProduccion = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.TipoProduccion)) ?? string.Empty,
            CentroProduccion = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.CentroProduccion)) ?? string.Empty,
            CalidadPlanta = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.CalidadPlanta)) ?? string.Empty,
            CierreVta = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.CierreVta)) ?? string.Empty,
            Posicion = ParsedRowValidator.ObtenerEntero(row.Fijos, nameof(config.Fijos.Posicion)) ?? 0,
            Material = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.Material)) ?? string.Empty,
            CantPreAsignado = ParsedRowValidator.ObtenerEntero(row.Fijos, nameof(config.Fijos.CantPreAsignado)) ?? 0,
            CantTransito = ParsedRowValidator.ObtenerEntero(row.Fijos, nameof(config.Fijos.CantTransito)) ?? 0,
            CantLote = ParsedRowValidator.ObtenerEntero(row.Fijos, nameof(config.Fijos.CantLote)) ?? 0,
            LoteExp = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.LoteExp)) ?? string.Empty,
            UbicacionEnAlmacen = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.UbicacionEnAlmacen)) ?? string.Empty,
            FechaContabilizacion = fechaContabilizacion,
            FechaFabricacion = fechaFabricacion,
            Certificadora = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.Certificadora)) ?? string.Empty,
            FAnalFcoQco = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.FAnalFcoQco)) ?? string.Empty,
            FAnalMicobiol = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.FAnalMicobiol)) ?? string.Empty,
            FvAnalFcoQco = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.FvAnalFcoQco)) ?? string.Empty,
            FvAnalMocobiol = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.FvAnalMocobiol)) ?? string.Empty
        };

        result.Fijos = fijos;

        // Copiar directamente los valores dinámicos
        result.ParametrosCalidad = row.ParametrosCalidad ?? new Dictionary<string, string>();
        result.OtrosValores = row.OtrosValores ?? new Dictionary<string, string>();

        return result;
    }
    //Siglo actual 
    public static string ObtenerAnioCompletoPorSigloActual(string anioCorto)
    {
        if (!int.TryParse(anioCorto, out int anioInt) || anioInt < 0 || anioInt > 99)
            throw new ArgumentException("El año corto no es válido.");

        int anioActual = DateTime.Now.Year;
        string sigloActual = (anioActual / 100).ToString(); // Ej: 2025 / 100 = 20 → "20"

        return sigloActual + anioCorto.PadLeft(2, '0'); // Asegura que "5" se vuelva "05"
    }
    
    
    public static string ObtenerAnioCompleto(string anioCorto)
    {
        if (!int.TryParse(anioCorto, out int anioInt))
            throw new ArgumentException("El año corto no es válido.");

        int anioActual = DateTime.Now.Year;
        int anioActualCorto = anioActual % 100;

        // Si el año corto es menor o igual al año actual, asumimos siglo actual
        int siglo = anioInt <= anioActualCorto ? anioActual - anioActualCorto : anioActual - anioActualCorto - 100;

        int anioCompleto = siglo + anioInt;
        return anioCompleto.ToString();
    }
    
    public static string GetFechaCorte(string serie, string fechaFabricacion, string anioCompleto, string fechaContabilizacion)
    {
        if (serie == "PH" || serie == "11" || serie == "16")
        {
            var fechaContabilizacionValue = ProcesarFechas(fechaContabilizacion, removerParentesis: true);
            var fechaFabricacionValue = ProcesarFechas(fechaFabricacion);

            if (!string.IsNullOrEmpty(fechaContabilizacionValue) && !string.IsNullOrEmpty(fechaFabricacionValue))
            {
                var formato = "dd/MM/yyyy";
                var cultura = CultureInfo.InvariantCulture;

                if (DateTime.TryParseExact(fechaContabilizacionValue, formato, cultura, DateTimeStyles.None, out var fechaContabilizacionDt) &&
                    DateTime.TryParseExact(fechaFabricacionValue, formato, cultura, DateTimeStyles.None, out var fechaFabricacionDt))
                {
                    // 🔹 Aquí haces la comparación
                    if (fechaContabilizacionDt > fechaFabricacionDt)
                    {
                        return fechaFabricacionValue; 
                    }
                    else
                    {
                        var fechaMenosUnAnio = fechaFabricacionDt.AddYears(-1);
                        return fechaMenosUnAnio.ToString("dd/MM/yyyy");
                    }
                }
            }

            return "Error al procesa fechas";
        }

        return ProcesarFechas(fechaFabricacion, anioCompleto: anioCompleto);
    }

    private static string ProcesarFechas(string textoFechas, string anioCompleto = "", bool removerParentesis = false)
    {
        if (string.IsNullOrWhiteSpace(textoFechas))
            return string.Empty;

        // 1. Preparar las partes
        var partes = textoFechas
            .Split(",", StringSplitOptions.TrimEntries)
            .Select(f =>
            {
                if (removerParentesis)
                {
                    var idx = f.IndexOf("(");
                    return idx > -1 ? f.Substring(0, idx) : f;
                }
                return f;
            })
            .ToList();

        // 2. Parsear
        var formato = "dd/MM";
        var cultura = CultureInfo.InvariantCulture;

        var fechasValidas = partes
            .Where(f => f.Contains("/") && f.Split('/').Length == 2)
            .Select(f =>
            {
                if (DateTime.TryParseExact(f, formato, cultura, DateTimeStyles.None, out var fecha))
                {
                    // Si se pasó un año explícito, usarlo
                    var year = !string.IsNullOrEmpty(anioCompleto) ? int.Parse(anioCompleto) : fecha.Year;
                    return new DateTime(year, fecha.Month, fecha.Day);
                }
                return (DateTime?)null;
            })
            .Where(f => f.HasValue)
            .Select(f => f.Value)
            .ToList();

        if (!fechasValidas.Any())
            return string.Empty;

        // 3. Tomar la menor fecha
        return fechasValidas.Min().ToString("dd/MM/yyyy");
    }
    
    

    public static ReporteLogisticDto MapearReporte(ParsedRowLogisticDto row, ExcelMappingLogisticsConfig config)
    {
        var result = new ReporteLogisticDto();

        // --- DEMANDA ---
        result.Demanda.Fijos = new DemandaFijas
        {
            Pos = ParsedRowValidator.ObtenerEntero(row.Demanda.Fijos, nameof(config.Demanda.Fijos.Pos)) ?? 0,
            Material = ParsedRowValidator.ObtenerTexto(row.Demanda.Fijos, nameof(config.Demanda.Fijos.Material)) ?? string.Empty,
            Descripcion = ParsedRowValidator.ObtenerTexto(row.Demanda.Fijos, nameof(config.Demanda.Fijos.Descripcion)) ?? string.Empty,
            CantidadAsignadaToneladas = ParsedRowValidator.ObtenerDouble(row.Demanda.Fijos, nameof(config.Demanda.Fijos.CantidadAsignadaToneladas)) ?? 0,
            UMVta = ParsedRowValidator.ObtenerTexto(row.Demanda.Fijos, nameof(config.Demanda.Fijos.UMVta)) ?? string.Empty,
            CantidadAsignadaSacos = ParsedRowValidator.ObtenerDouble(row.Demanda.Fijos, nameof(config.Demanda.Fijos.CantidadAsignadaSacos)) ?? 0,
            UMAlmac = ParsedRowValidator.ObtenerTexto(row.Demanda.Fijos, nameof(config.Demanda.Fijos.UMAlmac)) ?? string.Empty,
            Tolerancia = ParsedRowValidator.ObtenerDouble(row.Demanda.Fijos, nameof(config.Demanda.Fijos.Tolerancia)) ?? 0,
        };
        result.Demanda.ParamentrosCalidad = row.Demanda.ParametrosCalidad ?? new();

        foreach (var (codigo, ofertaDto) in row.Oferta)
        {
            // --- Fijos ---
            var ofertaFijos = new OfertaFijas
            {
                Pos = ParsedRowValidator.ObtenerEntero(ofertaDto.Fijos, nameof(config.Oferta.Fijos.Pos)) ?? 0,
                DescripcionMaterial = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.DescripcionMaterial)) ?? string.Empty,
                DescripcionCentro = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.DescripcionCentro)) ?? string.Empty,
                Lote = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.Lote)) ?? string.Empty,
                CantidadAsignadaSacos = ParsedRowValidator.ObtenerDouble(ofertaDto.Fijos, nameof(config.Oferta.Fijos.CantidadAsignadaSacos)) ?? 0,
                UMAlmac = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.UMAlmac)) ?? string.Empty,
                FechaCotizacion = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.FechaCotizacion)) ?? string.Empty,
                FechaFabricacion = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.FechaFabricacion)) ?? string.Empty,
                CantidadAsignadaToneladas = ParsedRowValidator.ObtenerDouble(ofertaDto.Fijos, nameof(config.Oferta.Fijos.CantidadAsignadaToneladas)) ?? 0,
                UMVta = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.UMVta)) ?? string.Empty,
                FechaAnalisisQuimico = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.FechaAnalisisQuimico)) ?? string.Empty,
                FechaVencimientoQuimico = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.FechaVencimientoQuimico)) ?? string.Empty,
                FechaAnalisisMicro = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.FechaAnalisisMicro)) ?? string.Empty,
                FechaVencimientoMicro = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.FechaVencimientoMicro)) ?? string.Empty,
                TipoAlmacen = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.TipoAlmacen)) ?? string.Empty,
                UbicacionAlmacen = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.UbicacionAlmacen)) ?? string.Empty,
            };

            // --- Parámetros calidad y otros ---
            var parametrosCalidad = ofertaDto.ParametrosCalidad ?? new();
            var otrosParametros = ofertaDto.OtrosParametros ?? new();

            result.Oferta[codigo] = new Oferta()
            {
                Fijos = ofertaFijos,
                ParametrosCalidad = parametrosCalidad,
                OtrosParamentros = otrosParametros
            };
        }

        result.Contrato = row.Contrato;
        result.PesoContenedores = row.PesoContenedores;

        return result;
    }
}
