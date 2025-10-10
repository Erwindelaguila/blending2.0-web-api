namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input;

public sealed record LogInpDemParametroDto
{
  public string CodigoParametro { get; init; } = default!;
  public decimal Valor { get; init; }
}
