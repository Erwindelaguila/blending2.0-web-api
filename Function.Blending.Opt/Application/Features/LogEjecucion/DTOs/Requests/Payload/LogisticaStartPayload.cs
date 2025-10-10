using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload;

public sealed record LogisticaStartPayload(string? Mensaje, string? NombreArchivo, string? UrlArchivo)
{
  public LogInpInfoDto? Info { get; init; }
  public LogInpFiltroDto? Filtro { get; init; }
  public LogInpDemandaDto? Demanda { get; init; }
  public IReadOnlyList<LogInpOfertaDto>? Oferta { get; init; }
}