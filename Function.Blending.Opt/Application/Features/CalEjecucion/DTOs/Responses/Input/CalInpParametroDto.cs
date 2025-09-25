using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Common;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses.Input;

public sealed class CalInpParametroDto
{
  public decimal Valor { get; set; }
  public BasicRefDto Calidad { get; set; } = default!;
  public BasicRefDto Parametro { get; set; } = default!;
}
