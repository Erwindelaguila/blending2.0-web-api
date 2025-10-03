namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input;

public sealed record LogInpOfeParametroDto
{
  public string CodigoParametro { get; init; } = default!;
  public decimal Valor { get; init; }
}
