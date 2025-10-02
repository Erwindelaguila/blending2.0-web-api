namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input;

public sealed record LogInpFilDivisionDto
{
  public string Ruma { get; init; } = default!;
  public string Division { get; init; } = default!;
}
