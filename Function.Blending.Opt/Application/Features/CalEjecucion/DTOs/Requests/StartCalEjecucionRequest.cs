using System;
using System.Collections.Generic;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests;

public sealed record StartCalEjecucionRequest(
  Guid PlantaId,
  string? Mensaje
)
{
  public CalInpFiltroDto? Filtro { get; init; }
  public IReadOnlyList<CalInpParametroDto>? Parametros { get; init; }
}
