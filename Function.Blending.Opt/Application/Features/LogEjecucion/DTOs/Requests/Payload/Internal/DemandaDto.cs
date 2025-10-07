namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload.Internal;

public sealed record DemandaDto(
    decimal Cantidad,
    IReadOnlyDictionary<string, decimal> Parametros
);
