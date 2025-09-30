using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload;

public sealed record LogisticaStartPayload(string? Mensaje)
{
  public LogInpInfoDto? Info { get; init; }
  public LogInpFiltroDto? Filtro { get; init; }
  public LogInpOfertaDto? Oferta { get; init; }
}