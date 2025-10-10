using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Converters;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;
using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests;

[JsonConverter(typeof(CompleteCalEjecucionRequestConverter))]
public sealed record CompleteCalEjecucionRequest
{
  public Guid Id { get; init; }
  public Guid EstadoId { get; init; }
  public string? Mensaje { get; init; }

  public IReadOnlyList<CalOutResumenDto>? Resumenes { get; init; }
  public IReadOnlyList<CalOutDetalleDto>? Detalles { get; init; }
}