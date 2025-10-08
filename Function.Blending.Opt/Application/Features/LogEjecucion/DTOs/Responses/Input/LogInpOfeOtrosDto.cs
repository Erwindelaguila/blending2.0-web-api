namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input;

public sealed record LogInpOfeOtrosDto
{
  public string Codigo { get; init; } = default!;
  public string? Valor { get; init; }
}
