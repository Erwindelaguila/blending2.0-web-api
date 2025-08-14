using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Models;
using Oferta = Function.Blending.Upload.Models.Oferta;

namespace Function.Blending.Upload.Helpers;

public static class ParsedRowMapperHelper
{
    public static RumaStockDisponibleDto Mapear(ParsedRowDto row, ExcelMappingConfig config)
    {
        var result = new RumaStockDisponibleDto();

        // Mapear columnas fijas desde config
        var fijos = new RumaValoresFijosDto
        {
            RumaNro = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.RumaNro)) ?? string.Empty,
            Cantidad = ParsedRowValidator.ObtenerEntero(row.Fijos, nameof(config.Fijos.Cantidad)) ?? 0, // Handle nullable value
            Um = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.Um)) ?? string.Empty,
            Codigo = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.Codigo)) ?? string.Empty,
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
            FechaContabilizacion = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.FechaContabilizacion)) ?? string.Empty,
            FechaFabricacion = ParsedRowValidator.ObtenerTexto(row.Fijos, nameof(config.Fijos.FechaFabricacion)) ?? string.Empty,
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

    public static ReporteLogisticDto MapearReporte(ParsedRowLogisticDto row, ExcelMappingLogisticsConfig config)
    {
        var result = new ReporteLogisticDto();

        // --- DEMANDA ---
        result.Demanda.Fijos = new DemandaFijas
        {
            Pos = ParsedRowValidator.ObtenerEntero(row.Demanda.Fijos, nameof(config.Demanda.Fijos.Pos)) ?? 0,
            Material = ParsedRowValidator.ObtenerTexto(row.Demanda.Fijos, nameof(config.Demanda.Fijos.Material)) ?? string.Empty,
            Descripcion = ParsedRowValidator.ObtenerTexto(row.Demanda.Fijos, nameof(config.Demanda.Fijos.Descripcion)) ?? string.Empty,
            CantidadAsignada = ParsedRowValidator.ObtenerDouble(row.Demanda.Fijos, nameof(config.Demanda.Fijos.CantidadAsignada)) ?? 0,
            UMVta = ParsedRowValidator.ObtenerTexto(row.Demanda.Fijos, nameof(config.Demanda.Fijos.UMVta)) ?? string.Empty,
            CantidadAsignadaTemp = ParsedRowValidator.ObtenerDouble(row.Demanda.Fijos, nameof(config.Demanda.Fijos.CantidadAsignadaTemp)) ?? 0,
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
                CantidadAsignada = ParsedRowValidator.ObtenerDouble(ofertaDto.Fijos, nameof(config.Oferta.Fijos.CantidadAsignada)) ?? 0,
                UMAlmac = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.UMAlmac)) ?? string.Empty,
                FechaCotizacion = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.FechaCotizacion)) ?? string.Empty,
                FechaFabricacion = ParsedRowValidator.ObtenerTexto(ofertaDto.Fijos, nameof(config.Oferta.Fijos.FechaFabricacion)) ?? string.Empty,
                CantidadAsignadaTemp = ParsedRowValidator.ObtenerDouble(ofertaDto.Fijos, nameof(config.Oferta.Fijos.CantidadAsignadaTemp)) ?? 0,
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
