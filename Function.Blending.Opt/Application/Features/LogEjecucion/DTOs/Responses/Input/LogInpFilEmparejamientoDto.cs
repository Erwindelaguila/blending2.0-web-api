namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input;

public sealed record LogInpFilEmparejamientoDto
{
  public string Grupo { get; init; } = default!;
  public string CodigoParametro { get; init; } = default!;
  public decimal Valor { get; init; }
}
