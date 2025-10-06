namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload.Internal;

public sealed record OfertaItemDto(
    string Lote,
    decimal CantidadAsignada,
    string DescripcionCentro,
    string UbicacionAlmacen,
    string Emparejamiento,
    IReadOnlyDictionary<string, decimal> Parametros
);
