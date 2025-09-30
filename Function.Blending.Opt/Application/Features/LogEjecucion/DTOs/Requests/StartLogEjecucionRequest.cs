using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests;

public sealed record StartLogEjecucionRequest
{
  public LogisticaStartPayload Start { get; init; } = null!;
  public LogisticaModelPayload Model { get; init; } = null!;
}