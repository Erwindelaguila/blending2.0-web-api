namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload.Internal;

public sealed record ObjetivoDto(
    decimal Cantidad,
    IReadOnlyDictionary<string, decimal> Parametros
);
