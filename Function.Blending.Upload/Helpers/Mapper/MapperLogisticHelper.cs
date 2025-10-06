using System.Globalization;
using Function.Blending.Upload.Helpers.Parsed;
using Function.Blending.Upload.Infrastructure.Config;
using Function.Blending.Upload.Infrastructure.Config.Input;
using Function.Blending.Upload.Models;
using Oferta = Function.Blending.Upload.Models.Oferta;

namespace Function.Blending.Upload.Helpers.Mapper;

public static class MapperLogisticHelper
{
    public static ExcelExtractLogisticDto Execute(ParsedRowLogisticDto row, ExcelMappingInputLogisticsConfig config)
    {
        var result = new ExcelExtractLogisticDto();

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
        result.PedidoVenta = row.PedidoVenta;
        result.FechaCarguio = ConvertirAFormatoIsoUtc(row.FechaCarguio);
        result.PlantaCodigo = row.PlantaCodigo;
        result.PlantaDescripcion = row.PlantaDescripcion;
        result.AlmacenCodigo = row.AlmacenCodigo;
        result.AlmacenDescripcion = row.AlmacenDescripcion;
        result.Cliente = row.Cliente;
        result.Asistente = row.Asistente;
        result.Supervisora = row.Supervisora;
        result.PaisDestino = row.PaisDestino;
        result.CantidadRuma = row.CantidadRuma;
        result.UnidadMedidaRuma = row.UnidadMedidaRuma;
        result.NumeroMovimientos = row.NumeroMovimientos;
        return result;
    }
    
    public static string ConvertirAFormatoIsoUtc(string fechaTexto)
    {
        if (string.IsNullOrWhiteSpace(fechaTexto))
            return string.Empty;
        // Parsear el string dd.MM.yyyy
        DateTime fecha = DateTime.ParseExact(
            fechaTexto,
            "dd.MM.yyyy",
            CultureInfo.InvariantCulture
        );

        // Dejar solo la parte de la fecha y hora 00:00:00
        fecha = fecha.Date;

        // Retornar en formato ISO 8601 UTC
        return fecha.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
    }
    
}
