namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses.Output;

public sealed class CalOutResParametroDto
{
  public Guid Id { get; init; }

  public Guid ResumenId { get; init; }

  public string CodigoParametro { get; init; } = default!;
  
  public decimal Valor { get; init; }
}
