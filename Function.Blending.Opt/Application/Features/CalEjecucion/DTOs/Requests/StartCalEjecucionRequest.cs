using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Payload;
using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests;

public sealed record StartCalEjecucionRequest
{

  [JsonPropertyName("start")]
  public CalidadStartPayload Start { get; init; } = null!;


  [JsonPropertyName("model")]
  public CalidadModelPayload Model { get; init; } = null!;
}
