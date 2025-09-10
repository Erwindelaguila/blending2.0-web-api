using System;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests
{
  // Record posicional, 1 solo tipo en el archivo (estilo Calidad)
  public sealed record StartLogEjecucionRequest(
    Guid PlantaId,
    string? Mensaje
  )
  {
    public LogInpInfoDto? Info { get; init; }
    public LogInpFiltroDto? Filtro { get; init; }
    public LogInpOfertaDto? Oferta { get; init; }
  }
}
