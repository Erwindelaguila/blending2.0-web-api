using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Payload;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests;

public sealed record StartCalEjecucionRequest
{
  public CalidadStartPayload? Start { get; init; }
  public CalidadModelPayload? Model { get; init; }
}
