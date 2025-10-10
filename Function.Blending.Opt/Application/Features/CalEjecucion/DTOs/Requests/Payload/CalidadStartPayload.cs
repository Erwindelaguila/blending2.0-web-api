using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Payload;

public sealed record CalidadStartPayload(
  Guid PlantaId,
  string? Mensaje,
  string? NombreArchivo,
  string? UrlArchivo
)
{
  public CalInpFiltroDto? Filtro { get; init; }
  public IReadOnlyList<CalInpParametroDto>? Parametros { get; init; }
}
