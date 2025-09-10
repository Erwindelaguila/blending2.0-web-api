namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

public sealed record CalOutDetParametroDto
{
  public string CodigoParametro { get; init; } = default!;
  public decimal Valor { get; init; }
}
