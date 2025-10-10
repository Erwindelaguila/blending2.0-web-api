namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

public sealed record CalOutDetOtrosDto
{
  public string Codigo { get; init; } = default!;
  public string Valor { get; init; } = default!;
}
