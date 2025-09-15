using System.Globalization;
using Function.Blending.Upload.Helpers.Parsed;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Infrastructure.Config.Input;
using Function.Blending.Upload.Models;

namespace Function.Blending.Upload.Helpers.Mapper;

public class MapperQualityHepler
{
    public static ExcelExtractQualityDto Execute(ParsedRowQualityDto rowQuality, ExcelMappingInputQualityConfig inputQualityConfig,
        List<CalidadDto> Calidades)
    {
        var result = new ExcelExtractQualityDto();

        // Tomamos el valor de RumaNro desde el row
        var rumaNro = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.RumaNro)) ?? string.Empty;
        var fechaFabricacion =
            ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.FechaFabricacion)) ??
            string.Empty;
        var fechaContabilizacion =
            ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.FechaContabilizacion)) ??
            string.Empty;

        var anioCorto = rumaNro.Length >= 5 ? rumaNro.Substring(3, 2) : string.Empty;

        var anioCompleto = ObtenerAnioCompleto(anioCorto);
        var serie = rumaNro.Length >= 7 ? rumaNro.Substring(5, 2) : string.Empty;

        var codigo = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.Codigo)) ?? string.Empty;


        var descripcionCalidad = Calidades
            .FirstOrDefault(c => c.CodigoMaterial == codigo)?.Descripcion ?? string.Empty;


        var nombreCalidad = Calidades
            .FirstOrDefault(c => c.CodigoMaterial == codigo)?.Nombre ?? string.Empty;

        var fijos = new ValoresFijosDto
        {
            RumaNro = rumaNro,
            // Los 3 primeros caracteres (si hay al menos 3)
            Planta = rumaNro.Length >= 3 ? rumaNro.Substring(0, 3) : string.Empty,
            Anio = anioCorto,
            // Los caracteres 6 y 7 (posición 5 y 6) si hay al menos 7
            Serie = serie,
            FechaCorte = GetFechaCorte(serie, fechaFabricacion, anioCompleto, fechaContabilizacion),
            DescripcionCalidad = descripcionCalidad,
            NombreCalidad = nombreCalidad,
            Cantidad = ParsedRowValidator.ObtenerDouble(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.Cantidad)) ?? 0,
            Um = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.Um)) ?? string.Empty,
            Codigo = codigo,
            DescripcionMaterial =
                ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.DescripcionMaterial)) ??
                string.Empty,
            CentroUbicacion = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.CentroUbicacion)) ??
                              string.Empty,
            AlmacenUbicacion =
                ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.AlmacenUbicacion)) ??
                string.Empty,
            TipoProduccion = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.TipoProduccion)) ??
                             string.Empty,
            CentroProduccion =
                ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.CentroProduccion)) ??
                string.Empty,
            CalidadPlanta = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.CalidadPlanta)) ??
                            string.Empty,
            CierreVta = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.CierreVta)) ??
                        string.Empty,
            Posicion = ParsedRowValidator.ObtenerEntero(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.Posicion)) ?? 0,
            Material = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.Material)) ?? string.Empty,
            CantPreAsignado =
                ParsedRowValidator.ObtenerEntero(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.CantPreAsignado)) ?? 0,
            CantTransito = ParsedRowValidator.ObtenerEntero(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.CantTransito)) ?? 0,
            CantLote = ParsedRowValidator.ObtenerEntero(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.CantLote)) ?? 0,
            LoteExp = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.LoteExp)) ?? string.Empty,
            UbicacionEnAlmacen =
                ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.UbicacionEnAlmacen)) ??
                string.Empty,
            FechaContabilizacion = fechaContabilizacion,
            FechaFabricacion = fechaFabricacion,
            Certificadora = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.Certificadora)) ??
                            string.Empty,
            FAnalFcoQco = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.FAnalFcoQco)) ??
                          string.Empty,
            FAnalMicobiol = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.FAnalMicobiol)) ??
                            string.Empty,
            FvAnalFcoQco = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.FvAnalFcoQco)) ??
                           string.Empty,
            FvAnalMocobiol = ParsedRowValidator.ObtenerTexto(rowQuality.Fijos, nameof(inputQualityConfig.Fijos.FvAnalMocobiol)) ??
                             string.Empty
        };

        result.Fijos = fijos;

        // Copiar directamente los valores dinámicos
        result.ParametrosCalidad = rowQuality.ParametrosCalidad ?? new Dictionary<string, string>();
        result.OtrosValores = rowQuality.OtrosValores ?? new Dictionary<string, string>();

        return result;
    }

    private static string ObtenerAnioCompletoPorSigloActual(string anioCorto)
    {
        if (!int.TryParse(anioCorto, out int anioInt) || anioInt < 0 || anioInt > 99)
            throw new ArgumentException("El año corto no es válido.");

        int anioActual = DateTime.Now.Year;
        string sigloActual = (anioActual / 100).ToString(); // Ej: 2025 / 100 = 20 → "20"

        return sigloActual + anioCorto.PadLeft(2, '0'); // Asegura que "5" se vuelva "05"
    }


    private static string ObtenerAnioCompleto(string anioCorto)
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

    private static string GetFechaCorte(string serie, string fechaFabricacion, string anioCompleto,
        string fechaContabilizacion)
    {
        if (serie == "PH" || serie == "11" || serie == "16")
        {
            var fechaContabilizacionValue = ProcesarFechas(fechaContabilizacion, removerParentesis: true);
            var fechaFabricacionValue = ProcesarFechas(fechaFabricacion);

            if (!string.IsNullOrEmpty(fechaContabilizacionValue) && !string.IsNullOrEmpty(fechaFabricacionValue))
            {
                var formato = "dd/MM/yyyy";
                var cultura = CultureInfo.InvariantCulture;

                if (DateTime.TryParseExact(fechaContabilizacionValue, formato, cultura, DateTimeStyles.None,
                        out var fechaContabilizacionDt) &&
                    DateTime.TryParseExact(fechaFabricacionValue, formato, cultura, DateTimeStyles.None,
                        out var fechaFabricacionDt))
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
}